using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.WinUI.Models
{
    public partial class Empleado : Persona
    {
        [ObservableProperty]
        private string usuario;

        [ObservableProperty]
        private string contrasena;

        [ObservableProperty]
        private string cargo;

        [ObservableProperty]
        private decimal sueldo;

        [ObservableProperty]
        private Rol rol;

        public string NombreEmpleado => $"Empleado: {NombreCompleto}";
    }
}