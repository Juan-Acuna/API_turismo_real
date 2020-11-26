using System;

namespace API_TurismoReal.Models
{
    public class Notificacion
    {
        public int Id_notificacion { get; set; } //PRIMARY KEY
        public String Contenido { get; set; }
        public char Visto { get; set; }
        public String Link { get; set; }
        public String Username { get; set; }
    }
}
