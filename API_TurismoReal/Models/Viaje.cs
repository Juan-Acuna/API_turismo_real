using System;

namespace Models
{
    public class Viaje
    {
        public int Id_viaje { get; set; } //PRIMARY KEY
        public DateTime Fecha { get; set; }
        public String Origen { get; set; }
        public String Destino { get; set; }
        public char Ida { get; set; }
        public int Id_chofer { get; set; }
        public char Confirmado { get; set; }
        public char Salida { get; set; }
        public DateTime Hora_salida { get; set; }
        public char Llegada { get; set; }
        public DateTime Hora_llegada { get; set; }
        public String Patente { get; set; }
        public int Id_reserva { get; set; }
    }
}
