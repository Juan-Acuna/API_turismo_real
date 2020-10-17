using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Models
{
    public class PersonaAcompanante
    {
        public Acompanante Acompanante { get; set; }
        public Persona Persona { get; set; }
    }
}
