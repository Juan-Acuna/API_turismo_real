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
            var m = Mensajes.RecuperarClave;
            m.AgregarDestinatario(correo);
            m.ConfigurarAsunto("rol", "admin");
            m.ConfigurarMensaje("rol", "admin");
            m.ConfigurarMensaje("usuario", "jacuna");
            m.ConfigurarMensaje("codigo", "AvWiiSmiyEsYhovHTyMC");
            m.ConfigurarMensaje("salt", Tools.EncriptarUrlCompatible("jacuna" + "AvWiiSmiyEsYhovHTyMC"));
            m.ConfigurarMensaje("fecha", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            var r = ClienteSmtp.Enviar(m);
            return Ok(new { Correo = correo, Resultado = r});
        }

        [HttpGet("recuperar/{email}")]
        public async Task<IActionResult> Recuperar([FromRoute]String email)
        {
            var lista = await cmd.Find<Persona>("Email", email.ToLower());
            if (lista.Count <= 0)
            {
                return BadRequest();
            }
            var p = lista[0];
            var u = (await cmd.Find<Usuario>("Rut", p.Rut))[0];
            var m = Mensajes.RecuperarClave;
            var reset = new ClaveReset
            {
                Codigo = Tools.CodigoAleatorio(p.Rut, 20),
                Fecha = DateTime.Now,
                Vencimiento = DateTime.Now.AddDays(1),
                Canjeado = '0',
                Username = u.Username
            };
            cmd.Insert(reset);
            String salt = Tools.EncriptarUrlCompatible(u.Username + reset.Codigo);
            m.AgregarDestinatario(p.Email,p.Nombres+" "+p.Apellidos);
            m.ConfigurarMensaje("username", u.Username);
            m.ConfigurarMensaje("codigo", reset.Codigo);
            m.ConfigurarMensaje("salt", salt);
            m.ConfigurarMensaje("fecha", reset.Fecha.ToShortDateString()+" "+reset.Fecha.ToShortTimeString());
            ClienteSmtp.Enviar(m);
            return Ok();
        }

        [HttpGet("restablecer/{username}/{codigo}.{salt}")]
        public async Task<IActionResult> Restablecer([FromRoute]String username, [FromRoute]String codigo, [FromRoute]String salt)
        {
            ClaveReset reset = new ClaveReset();
            var rlist = await cmd.Find<ClaveReset>("Username",(String)username.ToLower());
            bool encontrado = false;
            foreach (var r in rlist)
            {
                if (r.Codigo.Equals(codigo))
                {
                    encontrado = true;
                    reset = r;
                    break;
                }
            }
            if (!encontrado)
            {
                return NotFound("No existe");
            }
            if (reset.Canjeado == '1')
            {
                NotFound("ya se canjeo");
            }
            String sal = Tools.EncriptarUrlCompatible(username+codigo);
            if (!sal.Equals(salt))
            {
                NotFound("sal invalida");
            }
            reset.Canjeado = '1';
            cmd.Update(reset);
            Usuario u = await cmd.Get<Usuario>(username);
            Persona p = await cmd.Get<Persona>(u.Rut);
            return Ok(Tools.GenerarToken(u, p));
        }

        [HttpPost("asignacion/{username}/{idchofer}/{codigo}.{salt}")]
        public async Task<IActionResult> Recuperar([FromRoute]String username,[FromRoute]int idchofer, [FromRoute]String codigo, [FromRoute]String salt)
        {
            Asignacion asignacion = new Asignacion();
            var lista = await cmd.Find<Asignacion>("Id_chofer",idchofer);
            bool encontrado = false;
            foreach(var a in lista)
            {
                if (a.Codigo.Equals(codigo))
                {
                    encontrado = true;
                    asignacion = a;
                    break;
                }
            }
            if (!encontrado)
            {
                return NotFound();
            }
            if (asignacion.Id_estado != 1)
            {
                return NotFound();
            }
            String sal = Tools.EncriptarUrlCompatible(username + codigo);
            if (!sal.Equals(salt))
            {
                NotFound();
            }
            return Ok();
        }
        [Authorize(Roles = "1")]
        [HttpGet("metricas")]
        public async Task<IActionResult> Metricas()
        {
            try
            {
                var metricas = new ProxyMetricas();
                var clientes = await cmd.Find<Usuario>("Id_rol", 5);
                var trs = await cmd.GetAll<Transaccion>();
                var ms = await cmd.GetAll<Mantencion>();
                var rs = await cmd.GetAll<Reserva>();
                var cont = 0;
                foreach(var t in trs)
                {
                    if (t.Fecha.Month == DateTime.Now.Month)
                    {
                        cont++;
                    }
                }
                metricas.Transacciones = cont;
                cont = 0;
                var hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                foreach (var m in ms)
                {
                    var fecha = new DateTime(DateTime.Now.Year, m.Fecha.Month, DateTime.Now.Day);
                    if (DateTime.Compare(fecha, hoy) >= 0 && DateTime.Compare(fecha, hoy.AddMonths(3)) <= 0)
                    {
                        cont++;
                    }
                }
                metricas.Mantenciones = cont;
                cont = 0;
                foreach (var r in rs)
                {
                    var fecha = new DateTime(DateTime.Now.Year, r.Inicio_estadia.Month, DateTime.Now.Day);
                    if (DateTime.Compare(fecha, hoy) >= 0 && DateTime.Compare(fecha, hoy.AddMonths(3)) <= 0)
                    {
                        cont++;
                    }
                }
                metricas.Reservas = cont;
                var deptos = await cmd.GetAll<Departamento>();
                foreach (var d in deptos)
                {
                    metricas.Departamentos[d.Id_estado - 1]++;
                }
                metricas.Usuarios = clientes.Count;
                metricas.Conectados = SesionsManager.Sesiones.ClientesActivos; 
                return Ok(metricas);
            }
            catch(Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}