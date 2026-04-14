using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public partial class DetalleFactura : ObservableObject
    {
        [ObservableProperty] private int id;
        public Factura? Factura { get; set; }

        [ObservableProperty] private Producto producto = null!;
        [ObservableProperty] private int cantidad;
        [ObservableProperty] private decimal precioUnitario;

        public decimal Subtotal => Cantidad * PrecioUnitario;

        public string ProductoDisplay => Producto?.NombreCompleto ?? "—";
        public string PrecioDisplay => PrecioUnitario.ToString("C0");
        public string SubtotalDisplay => Subtotal.ToString("C0");
    }
}