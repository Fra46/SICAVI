using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace SICAVI.DAL.Models
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

        public string FechaDisplay => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string TotalDisplay => Total.ToString("C0");

        private decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (var item in Detalles)
                total += item.Subtotal;

            return total;
        }
    }
}