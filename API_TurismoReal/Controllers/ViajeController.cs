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
    public class ViajeController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);

        [Authorize(Roles = "1,3,5")]
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
            List<Viaje> v = await cmd.GetAll<Viaje>();
            if (v.Count > 0)
            {
                return Ok(v);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,3,5")]
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
            Viaje v = await cmd.Get<Viaje>(id);
            if (v != null)
            {
                return Ok(v);
            }
            return BadRequest();
        }
        [Authorize(Roles = "3,5")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Viaje v)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Insert(v))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "3,5")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Viaje v)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Update(v))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "3,5")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Viaje v)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(v))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}