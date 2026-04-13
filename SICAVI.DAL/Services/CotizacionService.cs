using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;

namespace SICAVI.DAL.Services
{
    public class CotizacionService
    {
        private readonly DbContext _db;

        public CotizacionService(DbContext db)
        {
            _db = db;
        }

        public List<Cotizacion> ObtenerTodas() =>
            _db.Set<Cotizacion>()
               .Include(c => c.Detalles)
               .ToList();

        public void Guardar(Cotizacion cotizacion)
        {
            if (cotizacion.Id == 0)
            {
                cotizacion.Fecha = DateTime.Now;
                _db.Set<Cotizacion>().Add(cotizacion);
            }
            else
            {
                    _db.Set<Cotizacion>().Update(cotizacion);
            }

            _db.SaveChanges();
        }

        public void Eliminar(Cotizacion cotizacion)
        {
            _db.Set<Cotizacion>().Remove(cotizacion);
            _db.SaveChanges();
        }

        public Venta ConvertirAVenta(Cotizacion cotizacion, string metodoPago, decimal tasaIva)
        {
            var venta = new Venta
            {
                Cliente = cotizacion.Cliente,
                Empleado = cotizacion.Empleado,
                MetodoPago = metodoPago,
                Fecha = DateTime.Now
            };

            foreach (var d in cotizacion.Detalles)
            {
                venta.Detalles.Add(new DetalleVenta
                {
                    Producto = d.Producto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                });
            }

            venta.RecalcularTotales(tasaIva);
            return venta;
        }
    }
}