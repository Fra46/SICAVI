using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using SICAVI.WinUI.ViewModels;
using SICAVI.WinUI.Views;
using System;
using System.Linq;

namespace SICAVI.Views
{
    public sealed partial class VentasView : Page
    {
        public VentasView()
        {
            InitializeComponent();
        }

        private async void NuevaVenta_Click(object sender, RoutedEventArgs e)
        {
            var vm = (VentasViewModel)DataContext;

            var productos = vm.Ventas
                .SelectMany(v => v.Detalles.Select(d => d.Producto))
                .DistinctBy(p => p.Id)
                .ToList();

            var dialog = new NuevaVentaDialog(
                productos: App.Services.GetProductos(),
                clientes: vm.Clientes.ToList(),
                tasaIva: vm.TasaIva)
            {
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.VentaResultante != null)
            {
                bool ok = vm.RegistrarVenta(dialog.VentaResultante);

                if (ok)
                {
                    var toast = new ContentDialog
                    {
                        Title = "✅ Venta registrada",
                        Content = $"Total: {dialog.VentaResultante.TotalDisplay}  •  " +
                                  $"Metodo: {dialog.VentaResultante.MetodoPago}",
                        CloseButtonText = "OK",
                        XamlRoot = XamlRoot
                    };
                    await toast.ShowAsync();
                }
            }
        }

        private async void VerDetalleVenta_DoubleTapped(
            object sender,
            Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var vm = (VentasViewModel)DataContext;
            if (vm.VentaSeleccionada == null) return;

            var venta = vm.VentaSeleccionada;

            var lineas = venta.Detalles
                .Select(d => $"  • {d.ProductoDisplay}  ×{d.Cantidad}  =  {d.SubtotalDisplay}");

            var cuerpo = string.Join("\n", lineas) +
                         $"\n\n{'─',30}" +
                         $"\nSubtotal:  {venta.Subtotal:C0}" +
                         $"\nIVA:       {venta.Iva:C0}" +
                         $"\nTOTAL:     {venta.TotalDisplay}" +
                         $"\n\nCliente:   {venta.ClienteDisplay}" +
                         $"\nEmpleado:  {venta.EmpleadoDisplay}" +
                         $"\nFecha:     {venta.FechaDisplay}" +
                         $"\nMetodo:    {venta.MetodoPago}";

            var dialog = new ContentDialog
            {
                Title = $"Detalle — Venta #{venta.Id}",
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = cuerpo,
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                        TextWrapping = TextWrapping.Wrap
                    },
                    MaxHeight = 400
                },
                CloseButtonText = "Cerrar",
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}