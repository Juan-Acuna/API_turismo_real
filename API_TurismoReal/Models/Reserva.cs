using System;

namespace API_TurismoReal.Models
{
    public class Reserva
    {
        public int Id_reserva { get; set; } //PRIMARY KEY
        public int Valor_total { get; set; }
        public int Valor_pagado { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Inicio_estadia { get; set; }
        public DateTime Fin_estadia { get; set; }
        public char Checkin { get; set; }
        public char Confirmado { get; set; }
        public char Checkout { get; set; }
        public String Desc_checkin { get; set; }
        public String Desc_checkout { get; set; }
        public int Multa_total { get; set; }
        public int Multa_pagado { get; set; }
        public int Pagos { get; set; }
        public int N_pago { get; set; }
        public String Username { get; set; }
        public int Id_depto { get; set; }
        public int Id_estado { get; set; }
    }
}
