using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Asignacion
    {
        public int Id_asignacion { get; set; }
        public String Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public int Id_estado { get; set; }
        public int Id_chofer { get; set; }
        public int Id_viaje { get; set; }
    }
}
