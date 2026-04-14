using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using static QuestPDF.Helpers.Colors;

namespace SICAVI.DAL.Models
{
    public partial class Factura : ObservableObject
    {
        [ObservableProperty] private int id;

        [ObservableProperty] private string numero = string.Empty;

        [ObservableProperty] private Venta? venta;

        [ObservableProperty] private Cliente? cliente;
        [ObservableProperty] private Empleado? empleado;
        [ObservableProperty] private DateTime fecha;
        [ObservableProperty] private string metodoPago = "Efectivo";
        [ObservableProperty] private decimal subtotal;
        [ObservableProperty] private decimal iva;
        [ObservableProperty] private decimal total;
        [ObservableProperty] private bool anulada;
        [ObservableProperty] private string? observaciones;

        public ObservableCollection<DetalleFactura> Detalles { get; set; } = new();

        public string ClienteDisplay => Cliente?.NombreCompleto ?? "Consumidor final";
        public string EmpleadoDisplay => Empleado?.NombreCompleto ?? "—";
        public string FechaDisplay => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string TotalDisplay => Total.ToString("C0");
        public string EstadoDisplay => Anulada ? "Anulada" : "Activa";

        public void RecalcularTotales(decimal tasaIva = 0.19m)
        {
            Subtotal = 0;
            foreach (var d in Detalles) Subtotal += d.Subtotal;
            Iva = Math.Round(Subtotal * tasaIva, 2);
            Total = Subtotal + Iva;
        }
    }
}