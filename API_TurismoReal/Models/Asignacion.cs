using System;

namespace API_TurismoReal.Models
{
    public class Asignacion
    {
        public int Id_asignacion { get; set; } //PRIMARY KEY
        public String Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public int Id_estado { get; set; }
        public int Id_chofer { get; set; }
        public int Id_viaje { get; set; }
        public String Username { get; set; }
    }
}
