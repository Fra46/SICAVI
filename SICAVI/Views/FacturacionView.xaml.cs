using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using SICAVI.WinUI.ViewModels;
using SICAVI.WinUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace SICAVI.Views
{
    public sealed partial class FacturacionView : Page
    {
        public FacturacionView() => InitializeComponent();

        private FacturacionViewModel VM => (FacturacionViewModel)DataContext;

        // ── Nueva factura independiente ────────────────────────────
        private async void NuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NuevaFacturaDialog(
                App.Services.GetProductos(),
                App.Services.GetClientes())
            { XamlRoot = XamlRoot };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.FacturaResultante != null)
            {
                var factura = VM.RegistrarIndependiente(dialog.FacturaResultante);
                if (factura != null)
                    await MostrarOpcionesPdf(factura);
            }
        }

        // ── Facturar desde venta existente ────────────────────────
        private async void FacturarDesdeVenta_Click(object sender, RoutedEventArgs e)
        {
            if (VM.VentasSinFactura.Count == 0)
            {
                await MostrarMensaje("Sin ventas pendientes",
                    "No hay ventas pendientes de facturar. Todas las ventas ya tienen factura.");
                return;
            }

            var dialog = new SeleccionarVentaDialog(VM.VentasSinFactura.ToList())
            { XamlRoot = XamlRoot };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.VentaSeleccionada != null)
            {
                var factura = VM.RegistrarDesdeVenta(dialog.VentaSeleccionada);
                if (factura != null)
                    await MostrarOpcionesPdf(factura);
            }
        }

        // ── Doble clic → opciones de la factura ───────────────────
        private async void VerDetalleFactura_DoubleTapped(
            object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (VM.FacturaSeleccionada == null) return;
            await MostrarOpcionesPdf(VM.FacturaSeleccionada);
        }

        // ── Panel de opciones: PDF / Correo / Detalle ─────────────
        private async Task MostrarOpcionesPdf(Factura factura)
        {
            var detalle = string.Join("\n", factura.Detalles
                .Select(d => $"  • {d.ProductoDisplay}  ×{d.Cantidad}  =  {d.SubtotalDisplay}")) +
                $"\n\n  Subtotal:  {factura.Subtotal:C0}" +
                $"\n  IVA:       {factura.Iva:C0}" +
                $"\n  TOTAL:     {factura.TotalDisplay}";

            var dialog = new ContentDialog
            {
                Title = $"Factura {factura.Numero}",
                Content = new ScrollViewer
                {
                    Content = new TextBlock
                    {
                        Text = detalle,
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                        TextWrapping = TextWrapping.Wrap
                    },
                    MaxHeight = 300
                },
                PrimaryButtonText = "Guardar PDF",
                SecondaryButtonText = "Enviar por correo",
                CloseButtonText = "Cerrar",
                XamlRoot = XamlRoot
            };

            var opcion = await dialog.ShowAsync();

            if (opcion == ContentDialogResult.Primary)
                await GuardarPdf(factura);
            else if (opcion == ContentDialogResult.Secondary)
                await EnviarCorreo(factura);
        }

        // ── Guardar PDF ───────────────────────────────────────────
        private async Task GuardarPdf(Factura factura)
        {
            var pdf = VM.GenerarPdf(factura);
            if (pdf == null) return;

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = $"Factura_{factura.Numero}"
            };
            picker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow!);
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteBytesAsync(file, pdf);
                await MostrarMensaje("PDF guardado", $"Factura guardada en:\n{file.Path}");
            }
        }

        // ── Enviar correo ─────────────────────────────────────────
        private async Task EnviarCorreo(Factura factura)
        {
            var pdf = VM.GenerarPdf(factura);
            if (pdf == null) return;

            var configService = new ConfiguracionService();
            configService.Cargar();

            if (configService.SMTPActual == null ||
                string.IsNullOrWhiteSpace(configService.SMTPActual.Usuario))
            {
                await MostrarMensaje("SMTP no configurado",
                    "Configura el servidor de correo en Configuración antes de enviar facturas.");
                return;
            }

            var emailDefault = factura.Cliente?.Correo ?? string.Empty;

            var input = new TextBox
            {
                PlaceholderText = "correo@ejemplo.com",
                Text = emailDefault,
                Width = 340
            };

            var confirmDialog = new ContentDialog
            {
                Title = "Enviar factura por correo",
                Content = new StackPanel
                {
                    Spacing = 8,
                    Children =
            {
                new TextBlock { Text = "Correo del destinatario:" },
                input
            }
                },
                PrimaryButtonText = "Enviar",
                CloseButtonText = "Cancelar",
                XamlRoot = XamlRoot
            };

            var r = await confirmDialog.ShowAsync();
            if (r != ContentDialogResult.Primary) return;

            VM.EnviarCorreo(factura, pdf, configService.SMTPActual, input.Text.Trim());
        }

        private async Task MostrarMensaje(string titulo, string texto)
        {
            var d = new ContentDialog
            {
                Title = titulo,
                Content = texto,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };
            await d.ShowAsync();
        }
    }
}