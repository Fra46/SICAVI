using Microsoft.UI.Xaml.Controls;
using SICAVI.DAL.Models;
using System.Collections.Generic;

namespace SICAVI.WinUI.Views
{
    public sealed partial class SeleccionarVentaDialog : ContentDialog
    {
        public List<Venta> Ventas { get; }
        public Venta? VentaSeleccionada { get; private set; }

        public SeleccionarVentaDialog(List<Venta> ventas)
        {
            InitializeComponent();
            Ventas = ventas;
        }

        private void VentasGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VentaSeleccionada = VentasGrid.SelectedItem as Venta;
        }
    }
}