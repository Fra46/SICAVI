using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SICAVI.DAL.Data;
using SICAVI.DAL.Models;
using SICAVI.DAL.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SICAVI.WinUI.ViewModels
{
    public partial class VentasViewModel : ObservableObject
    {
        private readonly VentaService _ventaService;
        private readonly ConfiguracionService _configService;

        public ObservableCollection<Venta> Ventas { get; set; } = new();
        public ObservableCollection<Cliente> Clientes { get; set; } = new();

        [ObservableProperty]
        private Venta? ventaSeleccionada;

        [ObservableProperty]
        private string mensajeError = string.Empty;

        [ObservableProperty]
        private bool hayError;

        [ObservableProperty]
        private string busqueda = string.Empty;

        public decimal TotalVentasHoy => Ventas
            .Where(v => v != null && !v.Anulada && v.Fecha.Date == DateTime.Today)
            .Sum(v => v.Total);

        public int CantidadVentasHoy => Ventas
            .Count(v => v != null && !v.Anulada && v.Fecha.Date == DateTime.Today);

        public decimal TasaIva => _configService?.ConfiguracionActual?.iva ?? 0.19m;

        public string TotalVentasHoyDisplay => TotalVentasHoy.ToString("C0");

        public VentasViewModel()
        {
            var context = App.Services.GetService<ConnectionContext>();
            _ventaService = new VentaService(context!);

            _configService = new ConfiguracionService();
            _configService.Cargar();

            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var ventas = _ventaService.ObtenerTodas();
                Ventas = new ObservableCollection<Venta>(ventas);

                var clientes = _ventaService.ObtenerClientes();
                Clientes = new ObservableCollection<Cliente>(clientes);

                OnPropertyChanged(nameof(Ventas));
                OnPropertyChanged(nameof(Clientes));
                OnPropertyChanged(nameof(TotalVentasHoy));
                OnPropertyChanged(nameof(CantidadVentasHoy));
                OnPropertyChanged(nameof(TotalVentasHoyDisplay));

                HayError = false;
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        public bool RegistrarVenta(Venta venta)
        {
            try
            {
                _ventaService.Registrar(venta);
                Ventas.Insert(0, venta);
                OnPropertyChanged(nameof(TotalVentasHoy));
                OnPropertyChanged(nameof(CantidadVentasHoy));
                HayError = false;
                return true;
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
                return false;
            }
        }

        [RelayCommand]
        private void AnularVenta()
        {
            if (VentaSeleccionada == null) return;

            try
            {
                _ventaService.Anular(VentaSeleccionada);

                CargarDatos();
                HayError = false;
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        [RelayCommand]
        private void Actualizar() => CargarDatos();

        private void MostrarError(string mensaje)
        {
            MensajeError = mensaje;
            HayError = true;
        }
    }
}
