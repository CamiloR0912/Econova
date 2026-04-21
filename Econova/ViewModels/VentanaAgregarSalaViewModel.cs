using System.Windows.Input;
using Econova.Core;
using System;

namespace Econova.ViewModels
{
    public class VentanaAgregarSalaViewModel : ObservableObject
    {
        private string _nombre;
        private int _capacidad;
        private string _mensajeError;

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public int Capacidad
        {
            get => _capacidad;
            set => SetProperty(ref _capacidad, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public ICommand GuardarCommand { get; }
        public ICommand CerrarCommand { get; }

        public event Action<string, int> SalaGuardada;
        public event Action SolicitudCierre;

        public VentanaAgregarSalaViewModel()
        {
            GuardarCommand = new RelayCommand(o => Guardar());
            CerrarCommand = new RelayCommand(o => Cerrar());
            Capacidad = 1; // Valor por defecto
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MensajeError = "El nombre de la sala es obligatorio.";
                return;
            }

            if (Capacidad <= 0)
            {
                MensajeError = "La capacidad debe ser mayor a 0.";
                return;
            }

            SalaGuardada?.Invoke(Nombre, Capacidad);
        }

        private void Cerrar()
        {
            SolicitudCierre?.Invoke();
        }
    }
}
