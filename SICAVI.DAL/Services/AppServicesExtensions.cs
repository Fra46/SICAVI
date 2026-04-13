using Microsoft.Extensions.DependencyInjection;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace SICAVI
{
    public static class AppServicesExtensions
    {
        public static List<Producto> GetProductos(this IServiceProvider provider)
        {
            var context = provider.GetRequiredService<ConnectionContext>();
            return context.Productos
                .Where(p => p.Stock > 0)
                .OrderBy(p => p.Nombre)
                .ToList();
        }
        public static List<Cliente> GetClientes(this IServiceProvider provider)
        {
            var context = provider.GetRequiredService<ConnectionContext>();
            return context.Clientes
                .OrderBy(c => c.Nombre)
                .ToList();
        }
    }
}