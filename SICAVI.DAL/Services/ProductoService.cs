using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using System.Collections.Generic;

namespace SICAVI.DAL.Services
{
    public class ProductoService
    {
        private readonly ConnectionContext _context;

        public ProductoService(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Producto> ObtenerTodos() =>
            DalExecutor.Execute(
                () => _context.Productos.ToList(),
                nameof(ObtenerTodos));

        public void Agregar(Producto producto) =>
            DalExecutor.Execute(() =>
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
            }, nameof(Agregar));

        public void Eliminar(Producto producto) =>
            DalExecutor.Execute(() =>
            {
                _context.Productos.Remove(producto);
                _context.SaveChanges();
            }, nameof(Eliminar));

        public void Actualizar(Producto producto) =>
            DalExecutor.Execute(() =>
            {
                _context.Productos.Update(producto);
                _context.SaveChanges();
            }, nameof(Actualizar));
    }
}