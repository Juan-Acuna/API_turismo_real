using API_TurismoReal.Conexiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API_TurismoReal
{
    public static class ClienteSmtp
    {
        public static readonly String Nombre = "Turismo Real";
        public static readonly String Correo = "notificaciones@turismoreal.xyz";
        public static readonly String Clave = Temp.CLAVE_CORREO;
        public static MailAddress DireccionEmail = new MailAddress(Correo, Nombre);
        static SmtpClient smtp = new SmtpClient
        {
            Host = "smtp.WebSiteLive.net",
            Port = 587,//465
            EnableSsl = true,
            Timeout = 20000,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(Correo, Clave)
        };

        public static object Enviar(MensajeCorreo mensaje)
        {
            try
            {
                MailMessage msg = new MailMessage(mensaje.De, mensaje.Para);
                msg.Subject = mensaje.Asunto;
                msg.Body = mensaje.Contenido;
                smtp.Send(msg);
                return new { Entrada = mensaje, Salida = msg };
            }
            catch (Exception e)
            {
                return new { Entrada = mensaje, Error = e };
            }
        }
    }
}
