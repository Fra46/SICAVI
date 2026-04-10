using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public partial class Producto : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string codigo = string.Empty;

        [ObservableProperty]
        private string nombre = string.Empty;

        [ObservableProperty]
        private string? categoria;

        [ObservableProperty]
        private string? descripcion;

        [ObservableProperty]
        private decimal precio;

        [ObservableProperty]
        private int stock;

        public string NombreCompleto => $"{codigo} - {nombre}";
    }
}