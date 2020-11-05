using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public static class ServerURLs
    {
        static String URL_BASE = "http://localhost:81/agencia";
        public static String FUNCIONES = URL_BASE + "/funciones";
        public static String VISTAS = URL_BASE + "/vistas";
        public static String DEPTOS = URL_BASE + "/deptos";
        public static String CUENTA = URL_BASE + "/cuenta";
        public static String PagarUrl(Acccion act, object idReserva, object idTransaccion, object monto)
        {
            return FUNCIONES+"/pagar.php?act="+act.ToString()+"&rs=" + idReserva.ToString() + "&tr=" + idTransaccion.ToString() + "&monto=" + monto.ToString();
        } 
    }
    public enum Acccion
    {
        repay,
        cancel
    }
}
