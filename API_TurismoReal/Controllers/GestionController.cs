using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using Conection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

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

        [HttpPost("nuevacuenta")]
        public async Task<IActionResult> NuevaClave([FromBody]dynamic data)
        {
            String username = data.Username;
            var u = await cmd.Get<Usuario>(username);
            u.Clave = Tools.CodigoAleatorio(username);
            await cmd.Update(u);
            return Ok();
        }
    }
}