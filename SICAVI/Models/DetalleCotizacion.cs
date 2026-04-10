using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.WinUI.Models
{
    public partial class DetalleCotizacion : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Producto producto;

        [ObservableProperty]
        private int cantidad;

        [ObservableProperty]
        private decimal precioUnitario;

        public decimal Subtotal => cantidad * precioUnitario;
    }
}