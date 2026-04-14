using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SICAVI.DAL.Models
{
    public partial class Cotizacion : ObservableObject
    {
        [ObservableProperty] private int id;
        [ObservableProperty] private string numero = string.Empty;
        [ObservableProperty] private Cliente? cliente;
        [ObservableProperty] private Empleado? empleado;
        [ObservableProperty] private DateTime fecha;
        [ObservableProperty] private DateTime? fechaVencimiento;
        [ObservableProperty] private string? observaciones;
        [ObservableProperty] private EstadoCotizacion estado = EstadoCotizacion.Pendiente;

        public ObservableCollection<DetalleCotizacion> Detalles { get; set; } = new();

        public decimal Subtotal => Detalles.Sum(d => d.Subtotal);
        public decimal Iva => Math.Round(Subtotal * 0.19m, 2);
        public decimal Total => Subtotal + Iva;

        public string ClienteDisplay => Cliente?.NombreCompleto ?? "Sin cliente";
        public string FechaDisplay => Fecha.ToString("dd/MM/yyyy");
        public string TotalDisplay => Total.ToString("C0");
        public string EstadoDisplay => Estado.ToString();
        public string NumeroDisplay => string.IsNullOrEmpty(Numero) ? $"COT-{Id:D4}" : Numero;
    }

    public enum EstadoCotizacion
    {
        Pendiente,
        Aceptada,
        Rechazada,
        Vencida
    }
}