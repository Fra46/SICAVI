using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SICAVI.WinUI.ViewModels;
using SICAVI.WinUI.Views;
using System;

namespace SICAVI.WinUI.Views
{
    public sealed partial class InventarioView : Page
    {
        public InventarioView()
        {
            InitializeComponent();
        }

        private async void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductoDialog();
            dialog.XamlRoot = this.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var vm = (InventarioViewModel)this.DataContext;
                vm.GuardarProducto(dialog.Producto);
            }
        }

        private async void EditarProducto_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var vm = (InventarioViewModel)this.DataContext;

            if (vm.ProductoSeleccionado == null) return;

            var dialog = new ProductoDialog(vm.ProductoSeleccionado)
            {
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                vm.ActualizarProducto(vm.ProductoSeleccionado, dialog.Producto);
            }
        }
    }
}
