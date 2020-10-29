using System;

namespace API_TurismoReal.Models
{
    public class Mantencion
    {
        public int Id_mantencion { get; set; } //PRIMARY KEY
        public DateTime Fecha { get; set; }
        public char Hecho { get; set; }
        public int Id_depto { get; set; }
        public String Username { get; set; }
        public int Id_tipo { get; set; }
    }
}
