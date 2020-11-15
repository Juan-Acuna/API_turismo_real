using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Models
{
    public class ProxyMetricas
    {
        public int[] Usuarios = new int[5];
        public int[] Conectados = new int[5];
        public int Transacciones { get; set; }
        public int Reservas { get; set; }
        public int Mantenciones { get; set; }
        public int[] Departamentos = new int[5];
        public ProxyMetricas()
        {
            Usuarios[0] = 0;
            Usuarios[1] = 0;
            Usuarios[2] = 0;
            Usuarios[3] = 0;
            Usuarios[4] = 0;
            Conectados[0] = 0;
            Conectados[1] = 0;
            Conectados[2] = 0;
            Conectados[3] = 0;
            Conectados[4] = 0;
            Departamentos[0] = 0;
            Departamentos[1] = 0;
            Departamentos[2] = 0;
            Departamentos[3] = 0;
            Departamentos[4] = 0;
        }
    }
}
