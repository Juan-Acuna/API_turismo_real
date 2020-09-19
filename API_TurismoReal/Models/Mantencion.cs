using System;

namespace Models
{
    public class Mantencion
    {
        public int Id_mantencion { get; set; } //PRIMARY KEY
        public DateTime Fecha { get; set; }
        public int Valor { get; set; }
        public int Id_depto { get; set; }
    }
}
