using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.WinUI.Models
{
    public partial class IngresoPeriodo : ObservableObject
    {
        [ObservableProperty]
        private string periodo;

        [ObservableProperty]
        private decimal totalIngresos;
    }
}