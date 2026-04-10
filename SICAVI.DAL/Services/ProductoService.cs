using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SICAVI.DAL.Services
{
    public class ProductoService
    {
        private readonly ConnectionContext _context;

        public ProductoService(ConnectionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ObservableCollection<Producto> Productos { get; set; } = new();

        public List<Producto> ObtenerTodos()
        {
            return _context.Productos.ToList();
        }

        public void Agregar(Producto producto)
        {
            _context.Productos.Add(producto);
            _context.SaveChanges();
        }

        public void Eliminar(Producto producto)
        {
            _context.Productos.Remove(producto);
            _context.SaveChanges();
        }

        public void Actualizar(Producto producto)
        {
            _context.Productos.Update(producto);
            _context.SaveChanges();
        }
    }
}