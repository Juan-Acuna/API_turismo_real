using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Models
{
    public class ProxyMetricas
    {
        public int Usuarios { get; set; }
        public int Conectados { get; set; }
        public int Transacciones { get; set; }
        public int Reservas { get; set; }
        public int Mantenciones { get; set; }
        public List<Departamento> Departamentos { get; set; }
    }
}
