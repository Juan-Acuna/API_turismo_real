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
            m.AgregarDestinatario(correo,CodigoAleatorio("19915954-2"));
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
            u.Clave = CodigoAleatorio(username);
            await cmd.Update(u);
            var m = Mensajes.RecuperarClave;
            m.AgregarDestinatario(p.Email,p.Nombres.Split(' ')[0]+" "+p.Apellidos.Split(' ')[0]);
            ClienteSmtp.Enviar(m);
            return Ok();
        }

        [HttpPost("nuevaclave")]
        public async Task<IActionResult> NuevaClave([FromBody]String username)
        {
            var u = await cmd.Get<Usuario>(username);
            u.Clave = CodigoAleatorio(username);
            await cmd.Update(u);
            return Ok();
        }

        private String CodigoAleatorio(String rut)
        {
            String clave = "";
            int suma = DateTime.Now.Millisecond;
            foreach(var c in rut)
            {
                suma += c;
            }
            var r = new Random(suma);
            for(int i=0;i<10;i++)
            {
                int l = 65 + r.Next(57);
                if(l >= 91 && l <= 96)
                {
                    l = 95;
                }
                clave += (char)l;
            }
            return clave;
        }
    }
}