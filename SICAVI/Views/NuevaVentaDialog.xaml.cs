using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SICAVI.WinUI.Views
{
    public sealed partial class NuevaVentaDialog : ContentDialog
    {
        public ObservableCollection<DetalleVenta> Detalles { get; } = new();
        public List<Producto> Productos { get; }
        public List<Cliente> Clientes { get; }

        private readonly decimal _tasaIva;

        public Venta? VentaResultante { get; private set; }

        public NuevaVentaDialog(
            List<Producto> productos,
            List<Cliente> clientes,
            decimal tasaIva = 0.19m)
        {
            InitializeComponent();

            Productos = productos;
            Clientes = clientes;
            _tasaIva = tasaIva;

            Detalles.CollectionChanged += (_, __) => RecalcularTotales();
        }


        private void ProductoCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void AgregarAlCarrito_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ProductoCombo.SelectedItem is not Producto producto)
            {
                MostrarError("Selecciona un producto antes de agregar.");
                return;
            }

            int cantidad = (int)CantidadBox.Value;
            if (cantidad <= 0)
            {
                MostrarError("La cantidad debe ser mayor a 0.");
                return;
            }

            foreach (var item in Detalles)
            {
                if (item.Producto.Id == producto.Id)
                {
                    item.Cantidad += cantidad;
                    RecalcularTotales();

                    var temp = Detalles[Detalles.IndexOf(item)];
                    Detalles.RemoveAt(Detalles.IndexOf(item));
                    Detalles.Insert(Detalles.Count, temp);
                    ErrorBar.IsOpen = false;
                    return;
                }
            }

            Detalles.Add(new DetalleVenta
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
            if (CarritoGrid.SelectedItem is DetalleVenta seleccionado)
                Detalles.Remove(seleccionado);
        }

        private void OnRegistrar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Detalles.Count == 0)
            {
                args.Cancel = true;
                MostrarError("Agrega al menos un producto al carrito.");
                return;
            }

            var metodoPago = (MetodoPagoCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Efectivo";
            var cliente = ClienteCombo.SelectedItem as Cliente;

            var venta = new Venta
            {
                Cliente = cliente,
                MetodoPago = metodoPago,
                Fecha = DateTime.Now
            };

            foreach (var d in Detalles)
                venta.Detalles.Add(d);

            venta.RecalcularTotales(_tasaIva);

            VentaResultante = venta;
        }


        private void RecalcularTotales()
        {
            decimal subtotal = 0;
            foreach (var d in Detalles)
                subtotal += d.Subtotal;

            decimal iva = Math.Round(subtotal * _tasaIva, 0);
            decimal total = subtotal + iva;

            SubtotalText.Text = subtotal.ToString("C0");
            IvaText.Text = iva.ToString("C0");
            TotalText.Text = total.ToString("C0");
        }

        private void MostrarError(string mensaje)
        {
            ErrorBar.Message = mensaje;
            ErrorBar.IsOpen = true;
        }
    }
}