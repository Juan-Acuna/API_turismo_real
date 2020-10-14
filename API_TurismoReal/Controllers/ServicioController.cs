using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using API_TurismoReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            List<Servicio> s = await cmd.GetAll<Servicio>();
            if (s.Count > 0)
            {
                return Ok(s);
            }
            return BadRequest();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Servicio s = await cmd.Get<Servicio>(id);
            if (s != null)
            {
                return Ok(s);
            }
            return BadRequest();
        }
        [Authorize(Roles = "3")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Servicio s)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Insert(s))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "3")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Servicio s)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Update(s))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "3")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Servicio s)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(s))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}