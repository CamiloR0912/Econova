using Econova.Models;

namespace Econova.Services
{
    public interface IDialogService
    {
        bool Confirmar(string mensaje, string titulo);
        void Informar(string mensaje, string titulo);
        bool ConfirmarReserva(string sala, string fechas, string cliente);
        void MostrarReservaExitosa(string sala, string fechas, string cliente);
        bool ConfirmarEliminarReserva(string sala, string cliente, string cedula, string entrada);
        bool EditarReserva(Reserva reserva);
        bool ConfirmarEliminarSala(string nombre, int capacidad);
        void MostrarConfirmacionExito(string mensaje, string titulo);
    }
}