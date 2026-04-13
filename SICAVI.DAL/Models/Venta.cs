using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace SICAVI.DAL.Models
{
    public partial class Venta : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Cliente? cliente;

        [ObservableProperty]
        private Empleado? empleado;

        [ObservableProperty]
        private DateTime fecha;

        [ObservableProperty]
        private string metodoPago = "Efectivo";

        /// Subtotal antes de IVA.
        [ObservableProperty]
        private decimal subtotal;

        /// Monto de IVA aplicado.
        [ObservableProperty]
        private decimal iva;

        /// Total con IVA.
        [ObservableProperty]
        private decimal total;

        [ObservableProperty]
        private bool anulada;

        public ObservableCollection<DetalleVenta> Detalles { get; set; } = new();

        public string ClienteDisplay => Cliente?.NombreCompleto ?? "Consumidor final";
        public string EmpleadoDisplay => Empleado?.NombreCompleto ?? "—";
        public string FechaDisplay => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string TotalDisplay => Total.ToString("C0");
        public string EstadoDisplay => anulada ? "Anulada" : "Activa";

        /// Recalcula subtotal, IVA y total a partir de los detalles.
        public void RecalcularTotales(decimal tasaIva = 0.19m)
        {
            subtotal = Detalles.Sum(d => d.Subtotal);
            iva = Math.Round(subtotal * tasaIva, 2);
            total = subtotal + iva;
        }
    }
}