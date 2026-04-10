using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using SICAVI.WinUI.Models;
using SICAVI.WinUI.Services;
using Microsoft.Extensions.DependencyInjection;
using SICAVI.WinUI.Data;

namespace SICAVI.WinUI.ViewModels
{
    public partial class InventarioViewModel : ObservableObject
    {
        private readonly ProductoService _productoService;

        public ObservableCollection<Producto> Productos { get; set; } = new();

        [ObservableProperty]
        private Producto productoSeleccionado;

        public InventarioViewModel()
        {
            var context = App.Services.GetService<ConnectionContext>();
            _productoService = new ProductoService(context);

            CargarProductos();
        }

        private void CargarProductos()
        {
            var lista = _productoService.ObtenerTodos();
            Productos = new ObservableCollection<Producto>(lista);
            OnPropertyChanged(nameof(Productos));
        }

        [RelayCommand]
        private void AgregarProducto()
        {
            var nuevo = new Producto
            {
                Codigo = "Nuevo",
                Nombre = "Producto",
                Precio = 0,
                Stock = 0
            };

            _productoService.Agregar(nuevo);
            Productos.Add(nuevo);
        }

        [RelayCommand]
        private void EliminarProducto()
        {
            if (ProductoSeleccionado != null)
            {
                _productoService.Eliminar(ProductoSeleccionado);
                Productos.Remove(ProductoSeleccionado);
            }
        }

        public void GuardarProducto(Producto producto)
        {
            _productoService.Agregar(producto);
            Productos.Add(producto);
        }

        public void ActualizarProducto(Producto original, Producto editado)
        {
            original.Codigo = editado.Codigo;
            original.Nombre = editado.Nombre;
            original.Precio = editado.Precio;
            original.Stock = editado.Stock;

            _productoService.Actualizar(original);
        }
    }
}