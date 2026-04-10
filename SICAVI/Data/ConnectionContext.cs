using Microsoft.EntityFrameworkCore;
using SICAVI.WinUI.Models;

namespace SICAVI.WinUI.Data
{
    public class ConnectionContext : DbContext
    {
        public ConnectionContext(DbContextOptions<ConnectionContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}