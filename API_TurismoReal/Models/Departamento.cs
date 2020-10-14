using System;

namespace API_TurismoReal.Models
{
    public class Departamento
    {
        public int Id_depto { get; set; } //PRIMARY KEY
        public String Nombre { get; set; }
        public String Direccion { get; set; }
        public int Arriendo { get; set; }
        public int Habitaciones { get; set; }
        public int Banos { get; set; }
        public float Mts_cuadrados { get; set; }
        public int Contribuciones { get; set; }
        public int Dividendo { get; set; }
        public int Id_localidad { get; set; }
        public int Id_estado { get; set; }
    }
}
