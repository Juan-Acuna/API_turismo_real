using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Models
{
    public class MensajeError
    {
        public String Error { get; set; }

        public MensajeError() { }
        public MensajeError(String mensaje)
        {
            Error = mensaje;
        }

        public static MensajeError Nuevo(String mensaje)
        {
            return new MensajeError(mensaje);
        }
    }
}
