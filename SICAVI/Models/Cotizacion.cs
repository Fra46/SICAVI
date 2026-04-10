using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace SICAVI.WinUI.Models
{
    public partial class Cotizacion : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Cliente cliente;

        [ObservableProperty]
        private Empleado empleado;

        [ObservableProperty]
        private DateTime fecha;

        public ObservableCollection<DetalleCotizacion> Detalles { get; set; } = new();

        public decimal Total => CalcularTotal();

        private decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (var item in Detalles)
                total += item.Subtotal;

            return total;
        }
    }
}