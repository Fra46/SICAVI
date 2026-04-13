using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SICAVI.Views;
using SICAVI.WinUI.Views;

namespace SICAVI.WinUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            NavView.SelectionChanged += NavView_SelectionChanged;

            // Pantalla inicial
            ContentFrame.Navigate(typeof(DashboardView));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;

            switch (selectedItem.Tag)
            {
                case "dashboard":
                    ContentFrame.Navigate(typeof(DashboardView));
                    break;

                case "ventas":
                    ContentFrame.Navigate(typeof(VentasView));
                    break;

                case "inventario":
                    ContentFrame.Navigate(typeof(InventarioView));
                    break;

                case "facturacion":
                    ContentFrame.Navigate(typeof(FacturacionView));
                    break;

                case "cotizaciones":
                    ContentFrame.Navigate(typeof(CotizacionView));
                    break;

                case "chatbot":
                    ContentFrame.Navigate(typeof(ChatBotView));
                    break;
            }
        }
    }
}