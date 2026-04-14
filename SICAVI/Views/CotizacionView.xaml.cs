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
    public sealed partial class CotizacionView : Page
    {
        public CotizacionView() => InitializeComponent();

        private CotizacionViewModel VM => (CotizacionViewModel)DataContext;

        private async void NuevaCotizacion_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NuevaCotizacionDialog(
                VM.Productos.ToList(),
                VM.Clientes.ToList())
            { XamlRoot = XamlRoot };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.CotizacionResultante != null)
                VM.Registrar(dialog.CotizacionResultante);
        }

        private async void ConvertirFactura_Click(object sender, RoutedEventArgs e)
        {
            if (VM.CotizacionSeleccionada == null)
            { await Aviso("Selecciona una cotización primero."); return; }

            if (VM.CotizacionSeleccionada.Estado == EstadoCotizacion.Aceptada)
            { await Aviso("Esta cotización ya fue convertida."); return; }

            var metodo = await PedirMetodoPago();
            if (metodo == null) return;

            VM.ConvertirAFactura(VM.CotizacionSeleccionada, metodo);
        }

        private async void ConvertirVentaFactura_Click(object sender, RoutedEventArgs e)
        {
            if (VM.CotizacionSeleccionada == null)
            { await Aviso("Selecciona una cotización primero."); return; }

            if (VM.CotizacionSeleccionada.Estado == EstadoCotizacion.Aceptada)
            { await Aviso("Esta cotización ya fue convertida."); return; }

            var metodo = await PedirMetodoPago();
            if (metodo == null) return;

            var (_, factura) = VM.ConvertirAVentaYFactura(VM.CotizacionSeleccionada, metodo);
            if (factura != null)
                await Aviso($"Venta y factura {factura.Numero} generadas. Stock descontado.");
        }

        private async void VerPdf_Click(object sender, RoutedEventArgs e)
        {
            if (VM.CotizacionSeleccionada == null)
            { await Aviso("Selecciona una cotización primero."); return; }

            var pdf = VM.GenerarPdf(VM.CotizacionSeleccionada);
            if (pdf == null) return;

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = $"Cotizacion_{VM.CotizacionSeleccionada.NumeroDisplay}"
            };
            picker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow!);
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
                await FileIO.WriteBytesAsync(file, pdf);
        }

        private async Task<string?> PedirMetodoPago()
        {
            var combo = new ComboBox { SelectedIndex = 0, Width = 280 };
            foreach (var m in new[] { "Efectivo", "Tarjeta débito", "Tarjeta crédito",
                                      "Nequi", "Daviplata", "Transferencia" })
                combo.Items.Add(m);

            var d = new ContentDialog
            {
                Title = "Método de pago",
                Content = new StackPanel
                {
                    Spacing = 8,
                    Children = {
                    new TextBlock { Text = "Selecciona el método de pago:" }, combo }
                },
                PrimaryButtonText = "Continuar",
                CloseButtonText = "Cancelar",
                XamlRoot = XamlRoot
            };

            var r = await d.ShowAsync();
            return r == ContentDialogResult.Primary ? combo.SelectedItem?.ToString() : null;
        }

        private async Task Aviso(string msg)
        {
            var d = new ContentDialog
            {
                Title = "Aviso",
                Content = msg,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };
            await d.ShowAsync();
        }
    }
}