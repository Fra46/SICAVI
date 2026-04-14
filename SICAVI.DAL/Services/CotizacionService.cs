using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;

namespace SICAVI.DAL.Services
{
    public class CotizacionService
    {
        private readonly ConnectionContext _context;
        private readonly FacturaService _facturaService;
        private readonly VentaService _ventaService;

        public CotizacionService(
            ConnectionContext context,
            FacturaService facturaService,
            VentaService ventaService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _facturaService = facturaService;
            _ventaService = ventaService;
        }

        public List<Cotizacion> ObtenerTodas() =>
            DalExecutor.Execute(() =>
                _context.Cotizaciones
                    .Include(c => c.Cliente)
                    .Include(c => c.Empleado)
                    .Include(c => c.Detalles).ThenInclude(d => d.Producto)
                    .OrderByDescending(c => c.Fecha)
                    .ToList(),
                nameof(ObtenerTodas));

        public Cotizacion Registrar(Cotizacion cotizacion) =>
            DalExecutor.Execute(() =>
            {
                cotizacion.Numero = GenerarNumero();
                cotizacion.Fecha = DateTime.Now;
                cotizacion.Estado = EstadoCotizacion.Pendiente;

                if (cotizacion.FechaVencimiento == null)
                    cotizacion.FechaVencimiento = DateTime.Now.AddDays(15);

                _context.Cotizaciones.Add(cotizacion);
                _context.SaveChanges();
                return cotizacion;
            }, nameof(Registrar));

        public void Actualizar(Cotizacion cotizacion) =>
            DalExecutor.Execute(() =>
            {
                _context.Cotizaciones.Update(cotizacion);
                _context.SaveChanges();
            }, nameof(Actualizar));

        public void Eliminar(Cotizacion cotizacion) =>
            DalExecutor.Execute(() =>
            {
                var db = _context.Cotizaciones.Find(cotizacion.Id)
                    ?? throw new InvalidOperationException("Cotizacion no encontrada.");
                _context.Cotizaciones.Remove(db);
                _context.SaveChanges();
            }, nameof(Eliminar));

        public Factura ConvertirAFactura(Cotizacion cotizacion, string metodoPago) =>
            DalExecutor.Execute(() =>
            {
                var factura = new Factura
                {
                    Cliente = cotizacion.Cliente,
                    Empleado = cotizacion.Empleado,
                    MetodoPago = metodoPago,
                    Observaciones = $"Generada desde cotizacion {cotizacion.NumeroDisplay}"
                };

                foreach (var d in cotizacion.Detalles)
                {
                    factura.Detalles.Add(new DetalleFactura
                    {
                        Producto = d.Producto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    });
                }

                var facturaRegistrada = _facturaService.Registrar(factura);

                var db = _context.Cotizaciones.Find(cotizacion.Id);
                if (db != null)
                {
                    db.Estado = EstadoCotizacion.Aceptada;
                    _context.SaveChanges();
                }

                return facturaRegistrada;
            }, nameof(ConvertirAFactura));

        public (Venta venta, Factura factura) ConvertirAVentaYFactura(
            Cotizacion cotizacion, string metodoPago) =>
            DalExecutor.Execute(() =>
            {
                var cot = _context.Cotizaciones
                    .Include(c => c.Cliente)
                    .Include(c => c.Empleado)
                    .Include(c => c.Detalles).ThenInclude(d => d.Producto)
                    .FirstOrDefault(c => c.Id == cotizacion.Id)
                    ?? throw new InvalidOperationException("Cotizacion no encontrada.");

                var venta = new Venta
                {
                    Cliente = cot.Cliente,
                    Empleado = cot.Empleado,
                    MetodoPago = metodoPago
                };

                foreach (var d in cot.Detalles)
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        Producto = d.Producto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    });
                }

                _ventaService.Registrar(venta);

                var factura = _facturaService.FacturarDesdeVenta(
                    venta, $"Generada desde cotizacion {cot.NumeroDisplay}");

                var db = _context.Cotizaciones.Find(cot.Id);
                if (db != null) { db.Estado = EstadoCotizacion.Aceptada; _context.SaveChanges(); }

                return (venta, factura);
            }, nameof(ConvertirAVentaYFactura));

        private string GenerarNumero()
        {
            var ultimo = _context.Cotizaciones
                .OrderByDescending(c => c.Id)
                .Select(c => c.Numero)
                .FirstOrDefault();

            int siguiente = 1;
            if (!string.IsNullOrEmpty(ultimo) &&
                ultimo.StartsWith("COT-") &&
                int.TryParse(ultimo[4..], out int num))
            {
                siguiente = num + 1;
            }

            return $"COT-{siguiente:D4}";
        }
    }
}