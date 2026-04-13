using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using SICAVI.DAL.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SICAVI.WinUI.ViewModels
{
    public partial class FacturacionViewModel : ObservableObject
    {
        private readonly FacturaService _facturaService;
        private readonly ConfiguracionService _configService;

        public ObservableCollection<Factura> Facturas { get; set; } = new();

        [ObservableProperty]
        private Factura? facturaSeleccionada;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private bool hayError;

        public FacturacionViewModel()
        {
            var context = App.Services.GetService<ConnectionContext>()!;
            _facturaService = new FacturaService(context);

            _configService = new ConfiguracionService();
            _configService.Cargar();

            CargarDatos();
        }

        public void CargarDatos()
        {
            try
            {
                var lista = _facturaService.ObtenerTodas();
                Facturas = new ObservableCollection<Factura>(lista);
                OnPropertyChanged(nameof(Facturas));
                HayError = false;
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        [RelayCommand]
        private void Actualizar() => CargarDatos();

        public void AnularFactura(Factura factura, string motivo)
        {
            try
            {
                _facturaService.Anular(factura, motivo);
                CargarDatos();
            }
            catch (Exception ex) { MostrarError(ex.Message); }
        }

        public string GuardarPdf(Factura factura)
        {
            var carpeta = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return _facturaService.GuardarPdf(factura, carpeta);
        }

        public async Task EnviarCorreoAsync(Factura factura)
        {
            var smtp = _configService.SMTPActual;
            var correo = factura.Venta?.Cliente?.Correo
                          ?? throw new InvalidOperationException("El cliente no tiene correo.");

            await _facturaService.EnviarPorCorreoAsync(
                factura, smtp,
                smtp.Usuario ?? "noreply@maihardware.com",
                correo);
        }

        private void MostrarError(string msg) { MensajeError = msg; HayError = true; }
    }
}