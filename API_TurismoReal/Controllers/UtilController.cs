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
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
        [HttpGet("disponible/{username}")]
        public async Task<IActionResult> Disponible([FromRoute]String username)
        {
            bool d = true;
            var u = await cmd.Get<Usuario>(username);
            if (u != null)
            {
                d = false;
            }
            return Ok(new { Disponible = d });
        }
    }
}