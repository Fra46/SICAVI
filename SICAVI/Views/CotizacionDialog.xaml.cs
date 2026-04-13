using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SICAVI.WinUI.Views
{
    public sealed partial class CotizacionDialog : ContentDialog
    {
        public ObservableCollection<DetalleCotizacion> Items { get; } = new();
        public List<Producto> Productos { get; }
        public List<Cliente> Clientes { get; }

        public Cotizacion? CotizacionResultante { get; private set; }

        public CotizacionDialog(List<Producto> productos, List<Cliente> clientes)
        {
            InitializeComponent();
            Productos = productos;
            Clientes = clientes;
            Items.CollectionChanged += (_, __) => ActualizarTotal();
        }

        private void Agregar_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ProductoCombo.SelectedItem is not Producto producto)
            {
                MostrarError("Selecciona un producto.");
                return;
            }
            int cantidad = (int)CantidadBox.Value;

            foreach (var item in Items)
            {
                if (item.Producto.Id == producto.Id)
                {
                    item.Cantidad += cantidad;
                    ActualizarTotal();
                    ErrorBar.IsOpen = false;
                    return;
                }
            }

            Items.Add(new DetalleCotizacion
            {
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            });
            ErrorBar.IsOpen = false;
            CantidadBox.Value = 1;
        }

        private void Quitar_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is DetalleCotizacion sel)
                Items.Remove(sel);
        }

        private void ActualizarTotal()
        {
            decimal total = Items.Sum(i => i.Subtotal);
            TotalText.Text = $"Total: {total:C0}";
        }

        private void OnGuardar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Items.Count == 0)
            {
                args.Cancel = true;
                MostrarError("Agrega al menos un producto.");
                return;
            }

            var cotizacion = new Cotizacion
            {
                Cliente = ClienteCombo.SelectedItem as Cliente,
                Empleado = null,
                Fecha = DateTime.Now
            };
            foreach (var item in Items)
                cotizacion.Detalles.Add(item);

            CotizacionResultante = cotizacion;
        }

        private void MostrarError(string msg)
        {
            ErrorBar.Message = msg;
            ErrorBar.IsOpen = true;
        }
    }
}