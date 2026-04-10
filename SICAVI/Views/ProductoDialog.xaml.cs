using Microsoft.UI.Xaml.Controls;
using SICAVI.WinUI.Models;
using System;

namespace SICAVI.WinUI.Views
{
    public sealed partial class ProductoDialog : ContentDialog
    {
        public Producto Producto { get; set; }

        public ProductoDialog(Producto? producto = null)
        {
            this.InitializeComponent();

            Producto = producto != null
                ? new Producto
                {
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Stock = producto.Stock
                }
                : new Producto();

            this.Loaded += (_, __) =>
            {
                foreach (var child in (Content as StackPanel).Children)
                {
                    if (child is TextBox tb)
                    {
                        tb.TextChanged += (_, __) =>
                        {
                            ErrorBar.IsOpen = false;
                        };
                    }
                }
            };

            this.DataContext = Producto;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string error = "";

            if (string.IsNullOrEmpty(Producto.Codigo))
            {
                error += "• El código es obligatorio\n";
                CodigoTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }

            if (string.IsNullOrWhiteSpace(Producto.Nombre))
                error += "• El nombre es obligatorio\n";

            if (Producto.Precio <= 0)
                error += "• El precio debe ser mayor a 0\n";

            if (Producto.Stock < 0)
                error += "• El stock no puede ser negativo\n";

            if (!string.IsNullOrEmpty(error))
            {
                args.Cancel = true;
                ErrorBar.Title = "Errores en el formulario\n";
                ErrorBar.Message = error.Trim();
                ErrorBar.IsOpen = true;

                return;
            }
        }
    }
}