using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public partial class Configuracion : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string nombreTienda;

        [ObservableProperty]
        public decimal iva;

        [ObservableProperty]
        private string impresoraPredeterminada;
    }
}