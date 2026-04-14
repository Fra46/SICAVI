using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace SICAVI.DAL.Services
{
    public class FacturaService
    {
        private readonly ConnectionContext _context;

        public FacturaService(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Factura> ObtenerTodas() =>
            DalExecutor.Execute(() =>
                _context.Facturas
                    .Include(f => f.Cliente)
                    .Include(f => f.Empleado)
                    .Include(f => f.Venta)
                    .Include(f => f.Detalles).ThenInclude(d => d.Producto)
                    .OrderByDescending(f => f.Fecha)
                    .ToList(),
                nameof(ObtenerTodas));

        public List<Factura> ObtenerActivas() =>
            DalExecutor.Execute(() =>
                _context.Facturas
                    .Include(f => f.Cliente)
                    .Include(f => f.Detalles).ThenInclude(d => d.Producto)
                    .Where(f => !f.Anulada)
                    .OrderByDescending(f => f.Fecha)
                    .ToList(),
                nameof(ObtenerActivas));

        public Factura Registrar(Factura factura) =>
            DalExecutor.Execute(() =>
            {
                factura.Numero = GenerarNumero();
                factura.Fecha = DateTime.Now;
                factura.RecalcularTotales();

                _context.Facturas.Add(factura);
                _context.SaveChanges();
                return factura;
            }, nameof(Registrar));

        public Factura FacturarDesdeVenta(Venta venta, string? observaciones = null) =>
            DalExecutor.Execute(() =>
            {
                var yaFacturada = _context.Facturas
                    .Any(f => EF.Property<int?>(f, "VentaId") == venta.Id && !f.Anulada);

                if (yaFacturada)
                    throw new InvalidOperationException(
                        $"La venta #{venta.Id} ya tiene una factura activa.");

                var factura = new Factura
                {
                    Numero = GenerarNumero(),
                    Venta = venta,
                    Cliente = venta.Cliente,
                    Empleado = venta.Empleado,
                    Fecha = DateTime.Now,
                    MetodoPago = venta.MetodoPago,
                    Observaciones = observaciones
                };

                foreach (var d in venta.Detalles)
                {
                    factura.Detalles.Add(new DetalleFactura
                    {
                        Producto = d.Producto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    });
                }

                factura.RecalcularTotales();
                _context.Facturas.Add(factura);
                _context.SaveChanges();
                return factura;
            }, nameof(FacturarDesdeVenta));


        public void Anular(Factura factura) =>
            DalExecutor.Execute(() =>
            {
                var db = _context.Facturas.Find(factura.Id)
                    ?? throw new InvalidOperationException("Factura no encontrada.");

                db.Anulada = true;
                _context.SaveChanges();
            }, nameof(Anular));

        public void EnviarPorCorreo(
            Factura factura,
            byte[] pdfBytes,
            ConfiguracionSMTP smtp,
            string destinatario)
        {
            DalExecutor.Execute(() =>
            {
                if (string.IsNullOrWhiteSpace(destinatario))
                    throw new InvalidOperationException("El correo del destinatario es obligatorio.");

                using var client = new SmtpClient(smtp.Servidor, smtp.Puerto)
                {
                    EnableSsl = smtp.UsarSSL,
                    Credentials = new NetworkCredential(smtp.Usuario, smtp.Contrasena)
                };

                using var adjunto = new Attachment(
                    new MemoryStream(pdfBytes),
                    $"Factura_{factura.Numero}.pdf",
                    "application/pdf");

                var mensaje = new MailMessage
                {
                    From = new MailAddress(smtp.Usuario, "Mai Hardware Store"),
                    Subject = $"Factura {factura.Numero} — Mai Hardware Store",
                    Body = $"Estimado/a {factura.ClienteDisplay},\n\n" +
                              $"Adjuntamos su factura {factura.Numero} por un total de {factura.TotalDisplay}.\n\n" +
                              $"Gracias por su compra.\n\nMai Hardware Store",
                    IsBodyHtml = false
                };

                mensaje.To.Add(destinatario);
                mensaje.Attachments.Add(adjunto);

                client.Send(mensaje);
            }, nameof(EnviarPorCorreo));
        }

        private string GenerarNumero()
        {
            var ultimo = _context.Facturas
                .OrderByDescending(f => f.Id)
                .Select(f => f.Numero)
                .FirstOrDefault();

            int siguiente = 1;
            if (!string.IsNullOrEmpty(ultimo) &&
                ultimo.StartsWith("FAC-") &&
                int.TryParse(ultimo[4..], out int num))
            {
                siguiente = num + 1;
            }

            return $"FAC-{siguiente:D4}";
        }
    }
}