using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SICAVI.WinUI.Views
{
    public sealed partial class NuevaCotizacionDialog : ContentDialog
    {
        public ObservableCollection<DetalleCotizacion> Detalles { get; } = new();
        public List<Producto> Productos { get; }
        public List<Cliente> Clientes { get; }

        private const decimal TasaIva = 0.19m;
        public Cotizacion? CotizacionResultante { get; private set; }

        public NuevaCotizacionDialog(List<Producto> productos, List<Cliente> clientes)
        {
            InitializeComponent();
            Productos = productos;
            Clientes = clientes;
            Detalles.CollectionChanged += (_, __) => RecalcularTotales();
        }

        private void AgregarAlCarrito_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ProductoCombo.SelectedItem is not Producto producto)
            { MostrarError("Selecciona un producto."); return; }

            int cantidad = (int)CantidadBox.Value;
            if (cantidad <= 0) { MostrarError("La cantidad debe ser mayor a 0."); return; }

            foreach (var item in Detalles)
            {
                if (item.Producto.Id == producto.Id)
                {
                    item.Cantidad += cantidad;
                    var idx = Detalles.IndexOf(item);
                    Detalles.RemoveAt(idx);
                    Detalles.Insert(idx, item);
                    ErrorBar.IsOpen = false;
                    return;
                }
            }

            Detalles.Add(new DetalleCotizacion
            {
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            });

            ErrorBar.IsOpen = false;
            CantidadBox.Value = 1;
        }

        private void QuitarDetalle_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (CarritoGrid.SelectedItem is DetalleCotizacion sel)
                Detalles.Remove(sel);
        }

        private void OnGuardar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Detalles.Count == 0)
            { args.Cancel = true; MostrarError("Agrega al menos un producto."); return; }

            var cotizacion = new Cotizacion
            {
                Cliente = ClienteCombo.SelectedItem as Cliente,
                FechaVencimiento = DateTime.Now.AddDays((int)DiasVencimiento.Value),
                Observaciones = ObsBox.Text.Trim()
            };

            foreach (var d in Detalles) cotizacion.Detalles.Add(d);
            CotizacionResultante = cotizacion;
        }

        private void RecalcularTotales()
        {
            decimal subtotal = Detalles.Sum(d => d.Subtotal);
            decimal iva = Math.Round(subtotal * TasaIva, 0);
            SubtotalText.Text = subtotal.ToString("C0");
            IvaText.Text = iva.ToString("C0");
            TotalText.Text = (subtotal + iva).ToString("C0");
        }

        private void MostrarError(string msg) { ErrorBar.Message = msg; ErrorBar.IsOpen = true; }
    }
}