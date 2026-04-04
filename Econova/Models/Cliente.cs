using System;

namespace Econova.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombres} {Apellidos}";
        public string Inicial => !string.IsNullOrEmpty(Nombres) ? Nombres[0].ToString().ToUpper() : "?";
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}
