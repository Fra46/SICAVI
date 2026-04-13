using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SICAVI.WinUI.ViewModels;
using System;
using System.Linq;

namespace SICAVI.Views
{
    public sealed partial class FacturacionView : Page
    {
        public FacturacionView()
        {
            InitializeComponent();
        }

        private async void AnularFactura_Click(object sender, RoutedEventArgs e)
        {
            var vm = (FacturacionViewModel)DataContext;
            if (vm.FacturaSeleccionada == null) return;

            var motivoBox = new TextBox { PlaceholderText = "Motivo de anulación..." };
            var dialog = new ContentDialog
            {
                Title = "Anular factura",
                Content = motivoBox,
                PrimaryButtonText = "Anular",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                vm.AnularFactura(vm.FacturaSeleccionada, motivoBox.Text.Trim());
        }

        private async void GuardarPdf_Click(object sender, RoutedEventArgs e)
        {
            var vm = (FacturacionViewModel)DataContext;
            if (vm.FacturaSeleccionada == null) return;

            try
            {
                var ruta = vm.GuardarPdf(vm.FacturaSeleccionada);
                await new ContentDialog
                {
                    Title = "✅ PDF guardado",
                    Content = ruta,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
        }

        private async void EnviarCorreo_Click(object sender, RoutedEventArgs e)
        {
            var vm = (FacturacionViewModel)DataContext;
            if (vm.FacturaSeleccionada == null) return;

            try
            {
                await vm.EnviarCorreoAsync(vm.FacturaSeleccionada);
                await new ContentDialog
                {
                    Title = "✅ Correo enviado",
                    Content = $"Factura enviada a {vm.FacturaSeleccionada.Venta?.Cliente?.Correo}",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error al enviar",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                }.ShowAsync();
            }
        }

        private async void VerDetalle_DoubleTapped(
            object sender,
            Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var vm = (FacturacionViewModel)DataContext;
            if (vm.FacturaSeleccionada == null) return;

            var f = vm.FacturaSeleccionada;
            var v = f.Venta;

            var lineas = v.Detalles
                .Select(d => $"  • {d.ProductoDisplay,-28} x{d.Cantidad,3}  {d.SubtotalDisplay,10}");

            var cuerpo =
                $"Factura:  {f.Numero}\n" +
                $"Fecha:    {f.FechaDisplay}\n" +
                $"Cliente:  {f.ClienteDisplay}\n" +
                $"{"─",40}\n" +
                string.Join("\n", lineas) +
                $"\n{"─",40}\n" +
                $"Subtotal: {v.Subtotal:C0}\n" +
                $"IVA:      {v.Iva:C0}\n" +
                $"TOTAL:    {v.TotalDisplay}\n" +
                (f.Anulada ? $"\n⚠ ANULADA — {f.MotivoAnulacion}" : "");

            await new ContentDialog
            {
                Title = $"Detalle — {f.Numero}",
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
            }.ShowAsync();
        }
    }
}