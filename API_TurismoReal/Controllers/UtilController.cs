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
            var u = await cmd.Get<Usuario>(username);
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
                var u = await cmd.Find<Persona>("email", email);
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
                bool d = true;
                var u = await cmd.Find<Localidad>("Nombre", nombre);
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
                bool d = true;
                var u = await cmd.Find<Departamento>("Nombre", nombre);
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
    }
}