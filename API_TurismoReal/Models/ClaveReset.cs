using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class ClaveReset
    {
        public int Id_reset { get; set; }
        public String Codigo { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Vencimiento { get; set; }
        public char Canjeado { get; set; }
        public String Username { get; set; }
    }
}
