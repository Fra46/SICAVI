using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SICAVI.DAL.Services
{
    public class VentaService
    {
        private readonly ConnectionContext _context;

        public VentaService(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Venta> ObtenerTodas()
        {
            try
            {
                return _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Empleado)
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
                    .OrderByDescending(v => v.Fecha)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al cargar las ventas.", ex);
            }
        }

        public void Registrar(Venta venta)
        {
            try
            {
                // Descontar stock de cada producto
                foreach (var detalle in venta.Detalles)
                {
                    var producto = _context.Productos.Find(detalle.Producto.Id);
                    if (producto == null)
                        throw new InvalidOperationException($"Producto '{detalle.Producto.Nombre}' no encontrado.");

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
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al registrar la venta.", ex);
            }
        }

        public void Anular(Venta venta)
        {
            try
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
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al anular la venta.", ex);
            }
        }

        public List<Cliente> ObtenerClientes()
        {
            try
            {
                return _context.Clientes.OrderBy(c => c.Nombre).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al cargar clientes.", ex);
            }
        }
    }
}
