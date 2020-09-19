using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            List<Reserva> r = await cmd.GetAll<Reserva>();
            if (r.Count > 0)
            {
                return Ok(r);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            Reserva r = await cmd.Get<Reserva>(id);
            if (r != null)
            {
                return Ok(r);
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Reserva r)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Insert(r))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,4,5")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Reserva r)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(r))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Reserva r)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Delete(r))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}