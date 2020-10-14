using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_TurismoReal.Models;

namespace API_TurismoReal.Controllers
{
    [Route("api/interno/[controller]")]
    [ApiController]
    public class GestionController : ControllerBase
    {
        MensajesEstandar Mensajes = MensajesEstandar.Instancia;
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        //[Authorize]
        [HttpPost("enviarcorreo/{correo}")]
        public async Task<IActionResult> EnviarCorreo([FromRoute]String correo)
        {//ESTE METODO ESTA CONFIGURADO PARA SER DE PRUEBAS, CORREGIR MAS ADELANTE
            var m = Mensajes.NotificarMulta;
            m.AgregarDestinatario(correo);
            m.ConfigurarAsunto("rol", "admin");
            m.ConfigurarMensaje("rol", "admin");
            m.ConfigurarMensaje("usuario", "jacuna");
            m.ConfigurarMensaje("codigo", "cod432jkgfdigo");
            m.ConfigurarMensaje("salt", "valeporsalt");
            var r = ClienteSmtp.Enviar(m);
            return Ok(new { Correo = correo, Resultado = r});
        }

        [HttpPost("Recuperar/{codigo}")]
        public async Task<IActionResult> Recuperar([FromRoute]String codigo)
        {
            //extrae rut de la persona desdde la tabla de "peticiones"
            String username="";
            var u = await cmd.Get<Usuario>(username);
            var p = await cmd.Get<Persona>(u.Rut);
            u.Clave = Tools.CodigoAleatorio(username);
            await cmd.Update(u);
            var m = Mensajes.RecuperarClave;
            m.AgregarDestinatario(p.Email,p.Nombres.Split(' ')[0]+" "+p.Apellidos.Split(' ')[0]);
            ClienteSmtp.Enviar(m);
            return Ok();
        }

        [HttpGet("activar/{username}/{codigo}${salt}")]
        public async Task<IActionResult> NuevaClave([FromRoute]String username, [FromRoute]String codigo, [FromRoute]String salt)
        {
            ClaveReset reset = new ClaveReset();
            var rlist = await cmd.Find<ClaveReset>("username",(String)username);
            foreach(var r in rlist)
            {
                if (r.Codigo.Equals(codigo))
                {
                    reset = r;
                    break;
                }
            }
            if (reset.Canjeado == '1')
            {
                NotFound();
            }
            String sal = Tools.Encriptar(username+codigo);
            if (!sal.Equals(salt))
            {
                NotFound();
            }
            Usuario u = await cmd.Get<Usuario>(username);
            Persona p = await cmd.Get<Persona>(u.Rut);
            return Ok(Tools.GenerarToken(u, p));//REDIRIGIR
        }
    }
}