using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly VentaService _ventaService;
        private readonly FacturaService _facturaService;
        private readonly ConfiguracionService _configService;

        public ObservableCollection<Cotizacion> Cotizaciones { get; set; } = new();

        [ObservableProperty]
        private Cotizacion? cotizacionSeleccionada;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private bool hayError;

        public decimal TasaIva => _configService?.ConfiguracionActual?.iva ?? 0.19m;

        public CotizacionViewModel()
        {
            var context = App.Services.GetService<SICAVI.DAL.Data.ConnectionContext>()!;
            _cotizacionService = new CotizacionService(context!);
            _ventaService = new VentaService(context);
            _facturaService = new FacturaService(context);
            _configService = new ConfiguracionService();
            _configService.Cargar();

            CargarDatos();
        }

        public void CargarDatos()
        {
            var lista = _cotizacionService.ObtenerTodas();
            Cotizaciones = new ObservableCollection<Cotizacion>(lista);
            OnPropertyChanged(nameof(Cotizaciones));
        }

        public void Guardar(Cotizacion cotizacion)
        {
            try
            {
                _cotizacionService.Guardar(cotizacion);
                CargarDatos();
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        [RelayCommand]
        private void Eliminar()
        {
            if (CotizacionSeleccionada == null) return;
            _cotizacionService.Eliminar(CotizacionSeleccionada);
            CargarDatos();
        }

        public Factura? ConvertirAFactura(Cotizacion cotizacion, string metodoPago)
        {
            try
            {
                var venta = _cotizacionService.ConvertirAVenta(cotizacion, metodoPago, TasaIva);
                _ventaService.Registrar(venta);
                var factura = _facturaService.CrearDesdeVenta(venta.Id);
                CargarDatos();
                HayError = false;
                return factura;
            }
            catch (Exception ex) { MostrarError(ex.Message); return null; }
        }

        [RelayCommand]
        private void Actualizar() => CargarDatos();

        private void MostrarError(string msg) { MensajeError = msg; HayError = true; }
    }
}