using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SICAVI.WinUI.ViewModels
{
    public class ProductoViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private string codigo;
        public string Codigo
        {
            get => codigo;
            set
            {
                codigo = value;
                ValidateCodigo();
                OnPropertyChanged(nameof(Codigo));
            }
        }

        private string nombre;
        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                ValidateNombre();
                OnPropertyChanged(nameof(Nombre));
            }
        }

        private decimal precio;
        public decimal Precio
        {
            get => precio;
            set
            {
                precio = value;
                ValidatePrecio();
                OnPropertyChanged(nameof(Precio));
            }
        }

        private int stock;
        public int Stock
        {
            get => stock;
            set
            {
                stock = value;
                ValidateStock();
                OnPropertyChanged(nameof(Stock));
            }
        }

        private void ValidateCodigo()
        {
            ClearErrors(nameof(Codigo));
            if (string.IsNullOrWhiteSpace(Codigo))
                AddError(nameof(Codigo), "El código es obligatorio");
        }

        private void ValidateNombre()
        {
            ClearErrors(nameof(Nombre));
            if (string.IsNullOrWhiteSpace(Nombre))
                AddError(nameof(Nombre), "El nombre es obligatorio");
        }

        private void ValidatePrecio()
        {
            ClearErrors(nameof(Precio));
            if (Precio <= 0)
                AddError(nameof(Precio), "Debe ser mayor a 0");
        }

        private void ValidateStock()
        {
            ClearErrors(nameof(Stock));
            if (Stock < 0)
                AddError(nameof(Stock), "No puede ser negativo");
        }

        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        private void AddError(string prop, string error)
        {
            if (!_errors.ContainsKey(prop))
                _errors[prop] = new List<string>();

            _errors[prop].Add(error);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
        }

        private void ClearErrors(string prop)
        {
            if (_errors.Remove(prop))
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
        }

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}