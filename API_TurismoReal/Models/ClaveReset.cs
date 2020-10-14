using System;

namespace API_TurismoReal.Models
{
    public class ClaveReset
    {
        public int Id_reset { get; set; } //PRIMARY KEY
        public String Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Vencimiento { get; set; }
        public char Canjeado { get; set; }
        public String Username { get; set; }
    }
}
