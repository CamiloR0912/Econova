using System;

namespace Econova.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Sala { get; set; }
        public string Cliente { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaEntradaDt { get; set; }
        public DateTime FechaSalidaDt { get; set; }
        public string FechaEntrada { get; set; }
        public string HoraEntrada { get; set; }
        public string FechaSalida { get; set; }
        public string HoraSalida { get; set; }
    }
}
