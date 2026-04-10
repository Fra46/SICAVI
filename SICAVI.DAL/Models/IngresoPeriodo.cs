using CommunityToolkit.Mvvm.ComponentModel;

namespace SICAVI.DAL.Models
{
    public partial class IngresoPeriodo : ObservableObject
    {
        [ObservableProperty]
        private string periodo;

        [ObservableProperty]
        private decimal totalIngresos;
    }
}