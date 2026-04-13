using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace SICAVI.DAL.Models
{
    public partial class Factura : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private int ventaId;

        [ObservableProperty]
        private Venta venta = null!;

        [ObservableProperty]
        private string numero = string.Empty;

        [ObservableProperty]
        private DateTime fechaEmision;

        [ObservableProperty]
        private bool anulada;

        [ObservableProperty]
        private string? motivoAnulacion;

        public string NumeroDisplay => numero;
        public string FechaDisplay => fechaEmision.ToString("dd/MM/yyyy HH:mm");
        public string EstadoDisplay => anulada ? "Anulada" : "Activa";
        public string TotalDisplay => venta?.TotalDisplay ?? "$0";
        public string ClienteDisplay => venta?.ClienteDisplay ?? "—";
    }
}