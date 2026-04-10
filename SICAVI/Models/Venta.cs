using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace SICAVI.WinUI.Models
{
    public partial class Venta : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Cliente cliente;

        [ObservableProperty]
        private Empleado empleado;

        [ObservableProperty]
        private DateTime fecha;

        [ObservableProperty]
        private string metodoPago;

        [ObservableProperty]
        private decimal total;

        public ObservableCollection<DetalleVenta> Detalles { get; set; } = new();
    }
}