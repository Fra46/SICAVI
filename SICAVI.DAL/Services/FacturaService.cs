using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using System.Net;
using System.Net.Mail;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using QRCoder;

namespace SICAVI.DAL.Services
{
    public class FacturaService
    {
        private readonly ConnectionContext _context;

        public FacturaService(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private string GenerarNumero()
        {
            var ultimo = _context.Facturas
                .OrderByDescending(f => f.Id)
                .FirstOrDefault();

            int numero = (ultimo?.Id ?? 0) + 1;

            return $"FAC-{DateTime.Now.Year}-{numero:D5}";
        }

        public Factura CrearDesdeVenta(int ventaId) =>
            DalExecutor.Execute(() =>
            {
                var venta = _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefault(v => v.Id == ventaId)
                    ?? throw new Exception("Venta no encontrada");

                var factura = new Factura
                {
                    VentaId = venta.Id,
                    Venta = venta,
                    Numero = GenerarNumero(),
                    FechaEmision = DateTime.Now,
                    Anulada = false
                };

                _context.Facturas.Add(factura);
                _context.SaveChanges();
                return factura;
            }, nameof(CrearDesdeVenta));

        public List<Factura> ObtenerTodas() =>
            DalExecutor.Execute(
                () => _context.Facturas
                    .Include(f => f.Venta)
                        .ThenInclude(v => v.Cliente)
                    .Include(f => f.Venta)
                        .ThenInclude(v => v.Detalles)
                            .ThenInclude(d => d.Producto)
                    .OrderByDescending(f => f.FechaEmision)
                    .ToList(),
                nameof(ObtenerTodas));

        public void Anular(Factura factura, string motivo) =>
            DalExecutor.Execute(() =>
            {
                var f = _context.Facturas.Find(factura.Id)
                    ?? throw new InvalidOperationException("Factura no encontrada.");
                f.Anulada = true;
                f.MotivoAnulacion = motivo;
                _context.SaveChanges();
            }, nameof(Anular));

        public byte[] GenerarPdf(Factura factura)
        {
            if (factura.Anulada)
                throw new InvalidOperationException("No se puede generar PDF de una factura anulada.");

            var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Row(row =>
                    {
                        row.ConstantItem(80).Image(logoPath);

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MAI HARDWARE STORE")
                                .Bold().FontSize(18);

                            col.Item().Text("NIT: 123456789-0");
                            col.Item().Text("Valledupar, Colombia");
                            col.Item().Text("Tel: 300 000 0000");
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"FACTURA")
                                .Bold().FontSize(16);

                            col.Item().Text($"N° {factura.Numero}");
                            col.Item().Text($"Fecha: {factura.FechaDisplay}");
                        });
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Container().Padding(5).Border(1).Column(c =>
                        {
                            c.Item().Text("Cliente").Bold();
                            c.Item().Text(factura.ClienteDisplay);
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Producto").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Cant").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Subtotal").Bold();
                            });

                            foreach (var d in factura.Venta.Detalles)
                            {
                                table.Cell().BorderBottom(1).Padding(5).Text(d.ProductoDisplay);
                                table.Cell().BorderBottom(1).Padding(5).AlignRight().Text(d.Cantidad.ToString());
                                table.Cell().BorderBottom(1).Padding(5).AlignRight().Text(d.SubtotalDisplay);
                            }
                        });

                        col.Item().AlignRight().Column(t =>
                        {
                            t.Item().Text($"Subtotal: {factura.Venta.Subtotal:C0}");
                            t.Item().Text($"IVA (19%): {factura.Venta.Iva:C0}");
                            t.Item().Text($"TOTAL: {factura.TotalDisplay}")
                                .Bold().FontSize(14);
                        });

                        col.Item().AlignCenter().PaddingTop(10).Height(80).Image(
                            GenerarQr($"Factura:{factura.Numero}|Total:{factura.TotalDisplay}")
                        );
                    });

                    page.Footer().AlignCenter().Column(col =>
                    {
                        col.Item().Text("Gracias por su compra").Italic();
                        col.Item().Text("Este documento es una representación gráfica de la factura");
                    });
                });
            }).GeneratePdf();
        }

        public string GuardarPdf(Factura factura, string carpeta)
        {
            var bytes = GenerarPdf(factura);
            var ruta = Path.Combine(carpeta, $"{factura.Numero}.pdf");
            File.WriteAllBytes(ruta, bytes);
            return ruta;
        }

        private byte[] GenerarQr(string contenido)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(5);
        }

        public async Task EnviarPorCorreoAsync(
            Factura factura,
            ConfiguracionSMTP smtp,
            string correoRemitente,
            string correoDestino)
        {
            if (factura.Anulada)
                throw new InvalidOperationException("No se puede enviar una factura anulada.");

            if (string.IsNullOrWhiteSpace(correoDestino))
                throw new InvalidOperationException("El cliente no tiene correo registrado.");

            var pdfBytes = GenerarPdf(factura);

            using var client = new SmtpClient(smtp.Servidor, smtp.Puerto)
            {
                EnableSsl = smtp.UsarSSL,
                Credentials = new NetworkCredential(smtp.Usuario, smtp.Contrasena)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(correoRemitente, "Mai Hardware Store"),
                Subject = $"Factura {factura.Numero} – Mai Hardware Store",
                Body = $"Adjunto encontrará su factura {factura.Numero}. Gracias por su compra.",
            };

            mail.To.Add(correoDestino);
            mail.Attachments.Add(new Attachment(
                new MemoryStream(pdfBytes),
                $"{factura.Numero}.pdf",
                "application/pdf"));

            await client.SendMailAsync(mail);
        }
    }
}