using System;

namespace Econova.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string Inicial => !string.IsNullOrEmpty(Nombre) ? Nombre[0].ToString().ToUpper() : "?";
        public string NombreConCapacidad => $"{Nombre} - Capacidad {Capacidad} personas";
        public override string ToString() => NombreConCapacidad;
    }
}
