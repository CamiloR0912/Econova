using System.Collections.Generic;

namespace Econova.Models
{
    /// <summary>
    /// RF-07: Modelo auxiliar que extiende Sala con estado de ocupación actual y reservas activas.
    /// </summary>
    public class SalaConEstado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public bool Ocupada { get; set; }
        public string Estado => Ocupada ? "Ocupada" : "Disponible";
        public string ColorEstado => Ocupada ? "#DC2626" : "#059669";
        public string ColorFondo => Ocupada ? "#FEF2F2" : "#F0FDF4";
        public string Inicial => !string.IsNullOrEmpty(Nombre) ? Nombre[0].ToString().ToUpper() : "?";
        public string ColorInicial => Ocupada ? "#FECACA" : "#BBF7D0";
        public List<Reserva> ReservasActivas { get; set; } = new List<Reserva>();
        public int TotalReservasActivas => ReservasActivas?.Count ?? 0;
        public string TextoReservas => TotalReservasActivas == 1 
            ? "1 reserva activa" 
            : $"{TotalReservasActivas} reservas activas";
    }
}
