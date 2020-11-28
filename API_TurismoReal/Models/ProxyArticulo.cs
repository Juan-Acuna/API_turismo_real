using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Models
{
    public class ProxyArticulo
    {
        public Articulo Articulo { get; set; }
        public int Depto { get; set; }
        public bool Asignado { get; set; }
    }
}
