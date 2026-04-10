using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public partial class ConfiguracionSMTP : ObservableObject
    {
        [ObservableProperty]
        private string servidor;

        [ObservableProperty]
        private int puerto;

        [ObservableProperty]
        private string usuario;

        [ObservableProperty]
        private string contrasena;

        [ObservableProperty]
        private bool usarSSL;
    }
}