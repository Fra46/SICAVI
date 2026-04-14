using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using SICAVI.DAL.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace SICAVI.WinUI.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        private readonly FacturaService _facturaService;
        private readonly VentaService _ventaService;
        private readonly PdfService _pdfService;

        public ObservableCollection<Factura> Facturas { get; set; } = new();
        public ObservableCollection<Venta> VentasSinFactura { get; set; } = new();

        [ObservableProperty] private Factura? facturaSeleccionada;
        [ObservableProperty] private string mensajeError = string.Empty;
        [ObservableProperty] private bool hayError;
        [ObservableProperty] private string mensajeExito = string.Empty;
        [ObservableProperty] private bool hayExito;

        public int TotalFacturasHoy => Facturas.Count(f => !f.Anulada && f.Fecha.Date == DateTime.Today);
        public decimal TotalHoy => Facturas.Where(f => !f.Anulada && f.Fecha.Date == DateTime.Today).Sum(f => f.Total);
        public string TotalHoyDisplay => TotalHoy.ToString("C0");

        public FacturacionViewModel()
        {
            var context = App.Services.GetRequiredService<ConnectionContext>();
            _facturaService = new FacturaService(context);
            _ventaService = new VentaService(context);
            _pdfService = new PdfService();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var facturas = _facturaService.ObtenerTodas();
                Facturas = new ObservableCollection<Factura>(facturas);

                // Ventas que aún no tienen factura activa
                var todasVentas = _ventaService.ObtenerTodas();
                var idsFacturados = facturas
                    .Where(f => !f.Anulada && f.Venta != null)
                    .Select(f => f.Venta!.Id)
                    .ToHashSet();

                VentasSinFactura = new ObservableCollection<Venta>(
                    todasVentas.Where(v => !v.Anulada && !idsFacturados.Contains(v.Id)));

                OnPropertyChanged(nameof(Facturas));
                OnPropertyChanged(nameof(VentasSinFactura));
                OnPropertyChanged(nameof(TotalFacturasHoy));
                OnPropertyChanged(nameof(TotalHoyDisplay));
                HayError = false;
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        public Factura? RegistrarDesdeVenta(Venta venta, string? obs = null)
        {
            try
            {
                var f = _facturaService.FacturarDesdeVenta(venta, obs);
                Facturas.Insert(0, f);
                VentasSinFactura.Remove(venta);
                RefrescarMetricas();
                MostrarExito($"Factura {f.Numero} generada correctamente.");
                return f;
            }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        public Factura? RegistrarIndependiente(Factura factura)
        {
            try
            {
                var f = _facturaService.Registrar(factura);
                Facturas.Insert(0, f);
                RefrescarMetricas();
                MostrarExito($"Factura {f.Numero} generada correctamente.");
                return f;
            }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        public byte[]? GenerarPdf(Factura factura)
        {
            try { return _pdfService.GenerarFactura(factura); }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        public void EnviarCorreo(Factura factura, byte[] pdf, ConfiguracionSMTP smtp, string destino)
        {
            try
            {
                _facturaService.EnviarPorCorreo(factura, pdf, smtp, destino);
                MostrarExito($"Factura enviada a {destino}.");
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        [RelayCommand]
        private void AnularFactura()
        {
            if (FacturaSeleccionada == null) return;
            try
            {
                _facturaService.Anular(FacturaSeleccionada);
                CargarDatos();
                MostrarExito("Factura anulada.");
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        [RelayCommand]
        private void Actualizar() => CargarDatos();

        private void RefrescarMetricas()
        {
            OnPropertyChanged(nameof(TotalFacturasHoy));
            OnPropertyChanged(nameof(TotalHoyDisplay));
        }

        private void MostrarError(string msg) { MensajeError = msg; HayError = true; HayExito = false; }
        private void MostrarExito(string msg) { MensajeExito = msg; HayExito = true; HayError = false; }
    }
}