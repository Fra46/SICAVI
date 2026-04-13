using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SICAVI.DAL.Services
{
    public class VentaService
    {
        private readonly ConnectionContext _context;

        public VentaService(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Venta> ObtenerTodas() =>
            DalExecutor.Execute(
                () => _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Empleado)
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
                    .OrderByDescending(v => v.Fecha)
                    .ToList(),
                nameof(ObtenerTodas));

        public List<Cliente> ObtenerClientes() =>
            DalExecutor.Execute(
                () => _context.Clientes.OrderBy(c => c.Nombre).ToList(),
                nameof(ObtenerClientes));

        public void Registrar(Venta venta) =>
            DalExecutor.Execute(() =>
            {
                foreach (var detalle in venta.Detalles)
                {
                    var producto = _context.Productos.Find(detalle.Producto.Id)
                        ?? throw new InvalidOperationException(
                            $"Producto '{detalle.Producto.Nombre}' no encontrado.");

                    if (producto.Stock < detalle.Cantidad)
                        throw new InvalidOperationException(
                            $"Stock insuficiente para '{producto.Nombre}'. " +
                            $"Disponible: {producto.Stock}, solicitado: {detalle.Cantidad}.");

                    producto.Stock -= detalle.Cantidad;
                    detalle.Producto = producto;
                }

                venta.Fecha = DateTime.Now;
                _context.Ventas.Add(venta);
                _context.SaveChanges();
            }, nameof(Registrar));

        public void Anular(Venta venta) =>
            DalExecutor.Execute(() =>
            {
                var ventaDb = _context.Ventas
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefault(v => v.Id == venta.Id)
                    ?? throw new InvalidOperationException("Venta no encontrada.");

                foreach (var detalle in ventaDb.Detalles)
                {
                    var producto = _context.Productos.Find(detalle.Producto.Id);
                    if (producto != null)
                        producto.Stock += detalle.Cantidad;
                }

                _context.Ventas.Remove(ventaDb);
                _context.SaveChanges();
            }, nameof(Anular));
    }
}