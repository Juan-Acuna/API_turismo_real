using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_TurismoReal.Models;

namespace API_TurismoReal.Controllers
{
    [Route("api/publico/[controller]")]
    [ApiController]
    public class UtilController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok();
        }
        [HttpGet("disponible/username/{username}")]
        public async Task<IActionResult> UsernameDisponible([FromRoute]String username)
        {
            bool d = true;
            var u = await cmd.Get<Usuario>(username.ToLower());
            if (u != null)
            {
                d = false;
            }
            return Ok(new { Disponible = d });
        }
        [HttpGet("disponible/rut/{rut}")]
        public async Task<IActionResult> RutDisponible([FromRoute]String rut)
        {
            bool d = true;
            var u = await cmd.Get<Persona>(rut);
            if (u != null)
            {
                d = false;
            }
            return Ok(new { Disponible = d });
        }
        [HttpGet("disponible/email/{email}")]
        public async Task<IActionResult> EmailDisponible([FromRoute]String email)
        {
            try
            {
                bool d = true;
                var u = await cmd.Find<Persona>("email", email.ToLower());
                if (u.Count > 0)
                {
                    d = false;
                }
                return Ok(new { Disponible = d });
            }
            catch(Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [HttpGet("disponible/localidad/{nombre}")]
        public async Task<IActionResult> LocalidadDisponible([FromRoute]String nombre)
        {
            try
            {
                String n = "";
                foreach(var l in nombre.Split(' '))
                {
                    n += " " + Tools.Capitalize(l);
                }
                n = n.TrimStart();
                bool d = true;
                var u = await cmd.Find<Localidad>("Nombre", n);
                if (u.Count > 0)
                {
                    d = false;
                }
                return Ok(new { Disponible = d });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [HttpGet("disponible/depto/{nombre}")]
        public async Task<IActionResult> DeptoDisponible([FromRoute]String nombre)
        {
            try
            {
                String n = "";
                foreach (var l in nombre.Split(' '))
                {
                    n += " " + Tools.Capitalize(l);
                }
                n = n.TrimStart();
                bool d = true;
                var u = await cmd.Find<Departamento>("Nombre", n);
                if (u.Count > 0)
                {
                    d = false;
                }
                return Ok(new { Disponible = d });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [HttpGet("disponible/articulo/{nombre}")]
        public async Task<IActionResult> ArticuloDisponible([FromRoute]String nombre)
        {
            try
            {
                bool d = true;
                var u = await cmd.Find<Articulo>("Nombre", nombre);
                if (u.Count > 0)
                {
                    d = false;
                }
                return Ok(new { Disponible = d });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [HttpGet("disponible/patente/{patente}")]
        public async Task<IActionResult> PatenteDisponible([FromRoute]String patente)
        {
            try
            {
                bool d = true;
                var u = await cmd.Get<Vehiculo>(patente.ToUpper());
                if (u!=null)
                {
                    d = false;
                }
                return Ok(new { Disponible = d });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [HttpPost("switch/{cod}")]
        public IActionResult Switch([FromRoute]String cod)
        {
            if (cod.Equals("codigo-supersecreto"))
            {
                return Ok(ConexionOracle.Switch());
            }
            return BadRequest();
        }
        [HttpGet("origen")]
        public IActionResult Origen()
        {
            return Ok(ConexionOracle.Origen);
        }
    }
}