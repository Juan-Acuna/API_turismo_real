using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Conection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

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
        [HttpGet("disponible/rut/{email}")]
        public async Task<IActionResult> EmailDisponible([FromRoute]String email)
        {
            bool d = true;
            var u = await cmd.Get<Persona>(email);
            if (u != null)
            {
                d = false;
            }
            return Ok(new { Disponible = d });
        }
    }
}