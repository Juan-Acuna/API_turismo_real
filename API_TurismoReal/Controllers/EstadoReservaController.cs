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
    public class EstadoReservaController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [Authorize(Roles = "1,2,4,5")]
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
            List<EstadoReserva> e = await cmd.GetAll<EstadoReserva>();
            if (e.Count > 0)
            {
                return Ok(e);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2,4,5")]
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
            EstadoReserva e = await cmd.Get<EstadoReserva>(id);
            if (e != null)
            {
                return Ok(e);
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]EstadoReserva e)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Insert(e))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,4,5")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]EstadoReserva e)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(e))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]EstadoReserva e)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Delete(e))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}