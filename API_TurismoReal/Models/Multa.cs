using System;

namespace Models
{
    public class Multa
    {
        public int Id_multa { get; set; } //PRIMARY KEY
        public int Valor { get; set; }
        public String Descripcion { get; set; }
        public char Pagado { get; set; }
        public int Id_tipo { get; set; }
        public int Id_reserva { get; set; }
    }
}
