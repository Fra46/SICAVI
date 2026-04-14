using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using SICAVI.DAL.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SICAVI.WinUI.ViewModels
{
    public partial class CotizacionViewModel : ObservableObject
    {
        private readonly CotizacionService _cotizacionService;
        private readonly PdfService _pdfService;
        private readonly VentaService _ventaService;

        public ObservableCollection<Cotizacion> Cotizaciones { get; set; } = new();
        public ObservableCollection<Cliente> Clientes { get; set; } = new();
        public ObservableCollection<Producto> Productos { get; set; } = new();

        [ObservableProperty] private Cotizacion? cotizacionSeleccionada;
        [ObservableProperty] private string mensajeError = string.Empty;
        [ObservableProperty] private bool hayError;
        [ObservableProperty] private string mensajeExito = string.Empty;
        [ObservableProperty] private bool hayExito;

        public CotizacionViewModel()
        {
            var context = App.Services.GetRequiredService<ConnectionContext>();
            var factSvc = new FacturaService(context);
            var ventaSvc = new VentaService(context);
            _cotizacionService = new CotizacionService(context, factSvc, ventaSvc);
            _ventaService = ventaSvc;
            _pdfService = new PdfService();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var context = App.Services.GetRequiredService<ConnectionContext>();
                Cotizaciones = new ObservableCollection<Cotizacion>(_cotizacionService.ObtenerTodas());
                Clientes = new ObservableCollection<Cliente>(context.Clientes.OrderBy(c => c.Nombre).ToList());
                Productos = new ObservableCollection<Producto>(context.Productos.Where(p => p.Stock > 0).ToList());

                OnPropertyChanged(nameof(Cotizaciones));
                OnPropertyChanged(nameof(Clientes));
                OnPropertyChanged(nameof(Productos));
                HayError = false;
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        public Cotizacion? Registrar(Cotizacion cotizacion)
        {
            try
            {
                var c = _cotizacionService.Registrar(cotizacion);
                Cotizaciones.Insert(0, c);
                MostrarExito($"Cotizacion {c.NumeroDisplay} creada.");
                return c;
            }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        public byte[]? GenerarPdf(Cotizacion cotizacion)
        {
            try { return _pdfService.GenerarCotizacion(cotizacion); }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        /// <summary>Convierte en factura (sin descontar stock).</summary>
        public Factura? ConvertirAFactura(Cotizacion cotizacion, string metodoPago)
        {
            try
            {
                var f = _cotizacionService.ConvertirAFactura(cotizacion, metodoPago);
                CargarDatos();
                MostrarExito($"Cotizacion convertida a factura {f.Numero}.");
                return f;
            }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        /// <summary>Convierte en venta + factura (descuenta stock).</summary>
        public (Venta? venta, Factura? factura) ConvertirAVentaYFactura(
            Cotizacion cotizacion, string metodoPago)
        {
            try
            {
                var (v, f) = _cotizacionService.ConvertirAVentaYFactura(cotizacion, metodoPago);
                CargarDatos();
                MostrarExito($"Cotizacion convertida a venta y factura {f.Numero}.");
                return (v, f);
            }
            catch (Exception ex) { MostrarError(ex.Message); return (null, null); }
        }

        [RelayCommand]
        private void EliminarCotizacion()
        {
            if (CotizacionSeleccionada == null) return;
            try
            {
                _cotizacionService.Eliminar(CotizacionSeleccionada);
                Cotizaciones.Remove(CotizacionSeleccionada);
                MostrarExito("Cotizacion eliminada.");
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        [RelayCommand]
        private void Actualizar() => CargarDatos();

        private void MostrarError(string msg) { MensajeError = msg; HayError = true; HayExito = false; }
        private void MostrarExito(string msg) { MensajeExito = msg; HayExito = true; HayError = false; }
    }
}