using Microsoft.EntityFrameworkCore;
using SICAVI.DAL.Models;

namespace SICAVI.DAL.Data
{
    public class ConnectionContext : DbContext
    {
        public ConnectionContext(DbContextOptions<ConnectionContext> options)
            : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetallesFactura { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<DetalleCotizacion> DetallesCotizacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Venta ──────────────────────────────────────────────
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente).WithMany()
                .HasForeignKey("ClienteId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Empleado).WithMany()
                .HasForeignKey("EmpleadoId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta).WithMany(v => v.Detalles)
                .HasForeignKey("VentaId").OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Producto).WithMany()
                .HasForeignKey("ProductoId").OnDelete(DeleteBehavior.Restrict);

            // ── Factura ────────────────────────────────────────────
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Venta).WithMany()
                .HasForeignKey("VentaId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente).WithMany()
                .HasForeignKey("ClienteId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Empleado).WithMany()
                .HasForeignKey("EmpleadoId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(d => d.Factura).WithMany(f => f.Detalles)
                .HasForeignKey("FacturaId").OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(d => d.Producto).WithMany()
                .HasForeignKey("ProductoId").OnDelete(DeleteBehavior.Restrict);

            // ── Cotizacion ─────────────────────────────────────────
            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Cliente).WithMany()
                .HasForeignKey("ClienteId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Empleado).WithMany()
                .HasForeignKey("EmpleadoId").IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DetalleCotizacion>()
                .HasOne(d => d.Cotizacion).WithMany(c => c.Detalles)
                .HasForeignKey("CotizacionId").OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleCotizacion>()
                .HasOne(d => d.Producto).WithMany()
                .HasForeignKey("ProductoId").OnDelete(DeleteBehavior.Restrict);

            // ── Tipos decimales ────────────────────────────────────
            foreach (var entity in new[] { "Producto", "DetalleVenta", "Venta",
                                           "DetalleFactura", "Factura",
                                           "DetalleCotizacion" })
            {
                
            }

            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.Numero).IsUnique();

            modelBuilder.Entity<Cotizacion>()
                .HasIndex(c => c.Numero).IsUnique();
        }
    }
}