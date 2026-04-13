using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SICAVI.WinUI.ViewModels;
using SICAVI.WinUI.Views;
using System;

namespace SICAVI.WinUI.Views
{
    public sealed partial class CotizacionView : Page
    {
        public CotizacionView()
        {
            InitializeComponent();
        }

        private async void NuevaCotizacion_Click(object sender, RoutedEventArgs e)
        {
            var vm = (CotizacionViewModel)DataContext;

            var dialog = new CotizacionDialog(
                App.Services.GetProductos(),
                App.Services.GetClientes())
            {
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.CotizacionResultante != null)
                vm.Guardar(dialog.CotizacionResultante);
        }

        private async void ConvertirAFactura_Click(object sender, RoutedEventArgs e)
        {
            var vm = (CotizacionViewModel)DataContext;
            if (vm.CotizacionSeleccionada == null) return;

            // Pedir método de pago
            var metodoPagoCombo = new ComboBox
            {
                SelectedIndex = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            foreach (var m in new[] { "Efectivo", "Tarjeta debito", "Tarjeta credito", "Nequi", "Daviplata", "Transferencia" })
                metodoPagoCombo.Items.Add(m);

            var dialog = new ContentDialog
            {
                Title = "Convertir a factura",
                Content = new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new TextBlock { Text = "Selecciona el método de pago:" },
                        metodoPagoCombo
                    }
                },
                PrimaryButtonText = "Confirmar",
                CloseButtonText = "Cancelar",
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            var metodoPago = metodoPagoCombo.SelectedItem?.ToString() ?? "Efectivo";
            var factura = vm.ConvertirAFactura(vm.CotizacionSeleccionada, metodoPago);

            if (factura != null)
            {
                await new ContentDialog
                {
                    Title = "✅ Factura generada",
                    Content = $"Se generó la {factura.Numero} por {factura.TotalDisplay}",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
        }
    }
}