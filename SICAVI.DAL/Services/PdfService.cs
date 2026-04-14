using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SICAVI.DAL.Models;

namespace SICAVI.DAL.Services
{
    public class PdfService
    {
        private const string NombreTienda = "Mai Hardware Store";
        private const string ColorAccent = "#1D6FA5";
        private const string ColorGris = "#F5F5F5";
        private const string ColorTextoSub = "#666666";

        public PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ── Factura ────────────────────────────────────────────────

        public byte[] GenerarFactura(Factura factura)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => CabeceraPdf(c, factura.Numero, "FACTURA", factura.FechaDisplay));
                    page.Content().Element(c => ContenidoDocumento(
                        c,
                        factura.ClienteDisplay,
                        factura.EmpleadoDisplay,
                        factura.MetodoPago,
                        factura.Observaciones,
                        factura.Detalles.Select(d => (d.ProductoDisplay, d.Cantidad, d.PrecioUnitario, d.Subtotal)).ToList(),
                        factura.Subtotal,
                        factura.Iva,
                        factura.Total));
                    page.Footer().Element(PiePdf);
                });
            }).GeneratePdf();
        }

        // ── Cotizacion ─────────────────────────────────────────────

        public byte[] GenerarCotizacion(Cotizacion cotizacion)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => CabeceraPdf(c, cotizacion.NumeroDisplay, "COTIZACION", cotizacion.FechaDisplay));
                    page.Content().Element(c =>
                    {
                        c.Column(col =>
                        {
                            ContenidoDocumento(
                                col.Item(),
                                cotizacion.ClienteDisplay,
                                cotizacion.Empleado?.NombreCompleto ?? "—",
                                "—",
                                cotizacion.Observaciones,
                                cotizacion.Detalles.Select(d => (d.ProductoDisplay, d.Cantidad, d.PrecioUnitario, d.Subtotal)).ToList(),
                                cotizacion.Subtotal,
                                cotizacion.Iva,
                                cotizacion.Total);

                            if (cotizacion.FechaVencimiento.HasValue)
                            {
                                col.Item().PaddingTop(12).Text(
                                    $"Esta cotizacion es valida hasta el {cotizacion.FechaVencimiento.Value:dd/MM/yyyy}.")
                                    .FontSize(9).Italic().FontColor(ColorTextoSub);
                            }
                        });
                    });
                    page.Footer().Element(PiePdf);
                });
            }).GeneratePdf();
        }

        // ── Bloques reutilizables ──────────────────────────────────

        private void CabeceraPdf(IContainer container, string numero, string tipo, string fecha)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(NombreTienda).FontSize(18).Bold().FontColor(ColorAccent);
                        c.Item().Text("Hardware y ferreteria").FontSize(9).FontColor(ColorTextoSub);
                    });

                    row.ConstantItem(160).Column(c =>
                    {
                        c.Item().AlignRight().Text(tipo).FontSize(16).Bold().FontColor(ColorAccent);
                        c.Item().AlignRight().Text(numero).FontSize(12).Bold();
                        c.Item().AlignRight().Text($"Fecha: {fecha}").FontSize(9).FontColor(ColorTextoSub);
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor(ColorAccent);
            });
        }

        private void ContenidoDocumento(
            IContainer container,
            string cliente, string empleado, string metodoPago,
            string? observaciones,
            List<(string producto, int cantidad, decimal precio, decimal subtotal)> items,
            decimal subtotal, decimal iva, decimal total)
        {
            container.Column(col =>
            {
                // Datos del cliente
                col.Item().PaddingTop(16).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Cliente").Bold().FontSize(9).FontColor(ColorTextoSub);
                        c.Item().Text(cliente).FontSize(11);
                    });
                    row.ConstantItem(160).Column(c =>
                    {
                        c.Item().Text("Vendedor").Bold().FontSize(9).FontColor(ColorTextoSub);
                        c.Item().Text(empleado).FontSize(11);
                        c.Item().PaddingTop(4).Text("Metodo de pago").Bold().FontSize(9).FontColor(ColorTextoSub);
                        c.Item().Text(metodoPago).FontSize(11);
                    });
                });

                // Tabla de items
                col.Item().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                    });

                    // Encabezado
                    static IContainer HeaderCell(IContainer c) =>
                        c.Background(ColorAccent).Padding(6);

                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("Producto").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).AlignCenter().Text("Cant.").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).AlignRight().Text("Precio unit.").Bold().FontColor("#FFFFFF");
                        h.Cell().Element(HeaderCell).AlignRight().Text("Subtotal").Bold().FontColor("#FFFFFF");
                    });

                    // Filas
                    bool par = false;
                    foreach (var item in items)
                    {
                        string bg = par ? "#FFFFFF" : ColorGris;
                        par = !par;

                        IContainer Cell(IContainer c) => c.Background(bg).Padding(6);

                        table.Cell().Element(Cell).Text(item.producto);
                        table.Cell().Element(Cell).AlignCenter().Text(item.cantidad.ToString());
                        table.Cell().Element(Cell).AlignRight().Text(item.precio.ToString("C0"));
                        table.Cell().Element(Cell).AlignRight().Text(item.subtotal.ToString("C0"));
                    }
                });

                // Totales
                col.Item().PaddingTop(12).AlignRight().Width(200).Column(totales =>
                {
                    FilaTotal(totales, "Subtotal", subtotal.ToString("C0"), false);
                    FilaTotal(totales, "IVA (19%)", iva.ToString("C0"), false);
                    totales.Item().LineHorizontal(0.5f).LineColor("#CCCCCC");
                    FilaTotal(totales, "TOTAL", total.ToString("C0"), true);
                });

                // Observaciones
                if (!string.IsNullOrWhiteSpace(observaciones))
                {
                    col.Item().PaddingTop(16).Column(obs =>
                    {
                        obs.Item().Text("Observaciones").Bold().FontSize(9).FontColor(ColorTextoSub);
                        obs.Item().Text(observaciones).FontSize(9);
                    });
                }
            });
        }

        private static void FilaTotal(ColumnDescriptor col, string label, string valor, bool negrita)
        {
            col.Item().Row(row =>
            {
                var labelText = row.RelativeItem().Text(label)
                    .FontSize(negrita ? 11 : 9)
                    .FontColor(negrita ? "#000000" : "#666666");
                if (negrita) labelText.Bold();

                var valorText = row.ConstantItem(90).AlignRight().Text(valor)
                    .FontSize(negrita ? 11 : 9);
                if (negrita) valorText.Bold();
            });
        }

        private void PiePdf(IContainer container)
        {
            container.PaddingTop(8).BorderTop(0.5f).BorderColor("#CCCCCC").Row(row =>
            {
                row.RelativeItem().Text(NombreTienda).FontSize(8).FontColor(ColorTextoSub);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Pagina ").FontSize(8).FontColor(ColorTextoSub);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" de ").FontSize(8).FontColor(ColorTextoSub);
                    text.TotalPages().FontSize(8);
                });
            });
        }
    }
}