using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace SICAVI.WinUI.Models
{
    public partial class Cliente : Persona
    {
        [ObservableProperty]
        private DateTime fechaRegistro;

        [ObservableProperty]
        private string estado;

        [ObservableProperty]
        private int cantidadVentas;

        public bool EsFrecuente => cantidadVentas >= 10;

        public string NombreCliente => $"Sr/a: {NombreCompleto}";
    }
}