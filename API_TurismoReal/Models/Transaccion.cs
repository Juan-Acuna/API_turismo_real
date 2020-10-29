using System;

namespace API_TurismoReal.Models
{
    public class Transaccion
    {
        public String Id_pago { get; set; } //PRIMARY KEY
        public long Monto { get; set; }
        public DateTime Fecha { get; set; }
        public char Listo { get; set; }
        public String Token { get; set; }
        public String Comentario { get; set; }
        public int Id_tipo { get; set; }
        public int Id_medio { get; set; }
        public String Username { get; set; }
        public int Id_reserva { get; set; }
    }
}
