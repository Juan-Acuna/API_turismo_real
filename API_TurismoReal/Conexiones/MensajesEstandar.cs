using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public class MensajesEstandar
    {
        static String ARecuperarClave = "Contraseña Reiniciada.";
        static String MRecuperarClave = "";
        static String AAsignacion = "Asignación de Viaje.";
        static String MAsignacion = "";
        static String ACheckIn = "Notificación de Check-In.";
        static String MCheckIn = "";
        static String ACheckOut = "Notificación de Check-Out.";
        static String MCheckOut = "";
        static String AViaje = "Notificacion de transporte.";
        static String MViaje = "";
        static String ATransaccion = "Notificacion de {accion}.";
        static String MTransaccion = "";
        static String AServicio = "Recordatorio de servicio.";
        static String MServicio = "";
        static String ABaja = "Su cuenta ha sido desactivada.";
        static String MBaja = "";
        static String AMulta = "Usted ha sido multado.";
        static String MMulta = "VALE POR UNA NOTIFICACION DE MULTA";
        static String AActivar = "Activacion de cuenta de {rol} en Turismo Real.";
        static String MActivar = "<div><h1>¡Bienvenido a turismo real!</h1><hr><p>Se ha creado una cuenta de {rol}"
        +" asociada a este correo electronico y para poder hacer uso de esta, es necesario que establezca una contraseña para iniciar sesión</p>"
        +"<p>Para establecer su contraseña haga click <a href=\"https://turismoreal.xyz/api/interno/gestion/activar/{usuario}/{codigo}${salt}\">Aquí</a>.</p></div>";

        MensajeCorreo recuperarClave = new MensajeCorreo(ClienteSmtp.DireccionEmail,ARecuperarClave, MRecuperarClave);
        MensajeCorreo asignacionChofer = new MensajeCorreo(ClienteSmtp.DireccionEmail, AAsignacion, MAsignacion);
        MensajeCorreo notificacionCheckIn = new MensajeCorreo(ClienteSmtp.DireccionEmail, ACheckIn, MCheckIn);
        MensajeCorreo notificacionCheckOut = new MensajeCorreo(ClienteSmtp.DireccionEmail, ACheckOut, MCheckOut);
        MensajeCorreo notificacionViaje = new MensajeCorreo(ClienteSmtp.DireccionEmail, AViaje, MViaje);
        MensajeCorreo notificarTransaccion = new MensajeCorreo(ClienteSmtp.DireccionEmail, ATransaccion, MTransaccion);
        MensajeCorreo notificarServicio = new MensajeCorreo(ClienteSmtp.DireccionEmail, AServicio, MServicio);
        MensajeCorreo notificarBaja = new MensajeCorreo(ClienteSmtp.DireccionEmail, ABaja, MBaja);
        MensajeCorreo notificarMulta = new MensajeCorreo(ClienteSmtp.DireccionEmail, AMulta, MMulta);
        MensajeCorreo activacionCuenta = new MensajeCorreo(ClienteSmtp.DireccionEmail, AActivar, MActivar);

        public MensajeCorreo RecuperarClave { get { return new MensajeCorreo(recuperarClave); } }
        public MensajeCorreo AsignacionChofer { get { return new MensajeCorreo(asignacionChofer); } }
        public MensajeCorreo NotificacionCheckIn { get { return new MensajeCorreo(notificacionCheckIn); } }
        public MensajeCorreo NotificacionCheckOut { get { return new MensajeCorreo(notificacionCheckOut); } }
        public MensajeCorreo NotificacionViaje { get { return new MensajeCorreo(notificacionViaje); } }
        public MensajeCorreo NotificarTransaccion { get { return new MensajeCorreo(notificarTransaccion); } }
        public MensajeCorreo NotificarServicio { get { return new MensajeCorreo(notificarServicio); } }
        public MensajeCorreo NotificarBaja { get { return new MensajeCorreo(notificarBaja); } }
        public MensajeCorreo NotificarMulta { get { return new MensajeCorreo(notificarMulta); } }
        public MensajeCorreo ActivacionCuenta { get { return new MensajeCorreo(activacionCuenta); } }

        public static MensajesEstandar Instancia = new MensajesEstandar();

        private MensajesEstandar()
        {
            /*RecuperarClave.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            AsignacionChofer.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificacionCheckIn.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificacionCheckOut.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificacionViaje.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificarTransaccion.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificarServicio.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificarBaja.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);
            NotificarMulta.AgregarRemitente(ClienteSmtp.Correo, ClienteSmtp.Nombre);*/
        }
    }
}
