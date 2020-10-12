using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public class MensajeCorreo
    {
        public MailAddress De { get; set; }        
        public MailAddress Para { get; set; }
        public String Asunto { get; set; }
        public String Contenido { get; set; }
        public MensajeCorreo(MensajeCorreo mensaje)
        {
            this.Asunto = mensaje.Asunto;
            this.Contenido = mensaje.Contenido;
            this.De = mensaje.De;
            this.Para = mensaje.Para;
        }
        public MensajeCorreo(String asunto, String mensaje)
        {
            this.Asunto = asunto;
            this.Contenido = mensaje;
        }
        public MensajeCorreo(MailAddress remitente, String asunto, String mensaje)
        {
            this.Asunto = asunto;
            this.Contenido = mensaje;
            this.De = remitente;
        }
        public void AgregarRemitente(String correo, String nombre = null)
        {
            if (nombre == null)
            {
                nombre = correo.Split('@')[0];
            }
            this.De = new MailAddress(correo, nombre);
        }
        public void AgregarDestinatario(String correo, String nombre = null)
        {
            if (nombre == null)
            {
                nombre = correo.Split('@')[0];
            }
            this.Para = new MailAddress(correo, nombre);
        }
    }
}
