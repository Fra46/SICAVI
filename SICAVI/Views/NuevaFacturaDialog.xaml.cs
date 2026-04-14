using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SICAVI.WinUI.Views
{
    public sealed partial class NuevaFacturaDialog : ContentDialog
    {
        public ObservableCollection<DetalleFactura> Detalles { get; } = new();
        public List<Producto> Productos { get; }
        public List<Cliente> Clientes { get; }

        private const decimal TasaIva = 0.19m;
        public Factura? FacturaResultante { get; private set; }

        public NuevaFacturaDialog(List<Producto> productos, List<Cliente> clientes)
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

            Detalles.Add(new DetalleFactura
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
            if (CarritoGrid.SelectedItem is DetalleFactura sel)
                Detalles.Remove(sel);
        }

        private void OnGenerar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Detalles.Count == 0)
            { args.Cancel = true; MostrarError("Agrega al menos un producto."); return; }

            var metodoPago = (MetodoPagoCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Efectivo";

            var factura = new Factura
            {
                Cliente = ClienteCombo.SelectedItem as Cliente,
                MetodoPago = metodoPago,
                Observaciones = ObsBox.Text.Trim()
            };

            foreach (var d in Detalles) factura.Detalles.Add(d);
            factura.RecalcularTotales(TasaIva);
            FacturaResultante = factura;
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