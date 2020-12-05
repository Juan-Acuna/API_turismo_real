using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public static class ServerURLs
    {
        static String URL_BASE          = "http://localhost/agencia";
        public static String FUNCIONES  = URL_BASE  + "/controladores";
        public static String VISTAS     = URL_BASE  + "/vistas";
        public static String DEPTOS     = VISTAS    + "/deptos";
        public static String CUENTA     = VISTAS    + "/cuenta";
        public static String GESTION    = VISTAS    + "/gestion";
        public static String GetRutaGestion(Gestion opcion)
        {
            return GESTION + "/index.php#"+opcion.ToString();
        }
        public static String PagarUrl(Accion act, object idReserva, object idTransaccion, object monto = null)
        {
            if(act== Accion.repay)
            {
                return FUNCIONES + "/preparacion-pago.php?rs=" + idReserva.ToString() + "&tr=" + idTransaccion.ToString();
            }
            else
            {
                return FUNCIONES + "/pagar.php?act=" + act.ToString() + "&rs=" + idReserva.ToString() + "&tr=" + idTransaccion.ToString() + "&monto=" + monto.ToString()+"&origen=api";
            }
        } 
    }
    public enum Accion
    {
        pay,
        repay,
        commit,
        cancel
    }
    public enum Gestion
    {
        Multas,
        Reservas,
        Servicios,
        Transporte,
        CheckIn,
        CheckOut,
        Mantenciones
    }
}
