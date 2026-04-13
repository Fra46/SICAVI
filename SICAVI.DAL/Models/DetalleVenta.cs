using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public partial class DetalleVenta : ObservableObject
    {
        [ObservableProperty]
        private int id;

        public Venta? Venta { get; set; }

        [ObservableProperty]
        private Producto producto = null!;

        [ObservableProperty]
        private int cantidad;

        [ObservableProperty]
        private decimal precioUnitario;

        public decimal Subtotal => Cantidad * PrecioUnitario;

        public string ProductoDisplay => Producto?.NombreCompleto ?? "—";
        public string SubtotalDisplay => Subtotal.ToString("C0");
        public string PrecioDisplay => PrecioUnitario.ToString("C0");
    }
}