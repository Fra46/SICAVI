using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Models;

namespace SICAVI.DAL.Data
{
    public class ConnectionContext : DbContext
    {
        public ConnectionContext(DbContextOptions<ConnectionContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Venta → Cliente (Venta rápida sin cliente)
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey("ClienteId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Venta → Empleado
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Empleado)
                .WithMany()
                .HasForeignKey("EmpleadoId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // DetalleVenta → Venta
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey("VentaId")
                .OnDelete(DeleteBehavior.Cascade);

            // DetalleVenta → Producto
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey("ProductoId")
                .OnDelete(DeleteBehavior.Restrict);

            // Precio como decimal(18,2)
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Venta>()
                .Property(v => v.Iva)
                .HasColumnType("decimal(18,2)");
        }
    }
}