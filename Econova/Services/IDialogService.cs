namespace Econova.Services
{
    public interface IDialogService
    {
        bool Confirmar(string mensaje, string titulo);
        void Informar(string mensaje, string titulo);
        bool ConfirmarReserva(string sala, string fechas, string cliente);
        void MostrarReservaExitosa(string sala, string fechas, string cliente); // 👈 nuevo
    }
}