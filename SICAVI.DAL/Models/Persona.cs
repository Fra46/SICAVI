using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public abstract partial class Persona : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string apellido;

        [ObservableProperty]
        private string telefono;

        [ObservableProperty]
        private string correo;

        public string NombreCompleto => $"{nombre} {apellido}";
    }
}