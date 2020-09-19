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
    public class MedioPagoController : ControllerBase
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
            List<MedioPago> m = await cmd.GetAll<MedioPago>();
            if (m.Count > 0)
            {
                return Ok(m);
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
            MedioPago m = await cmd.Get<MedioPago>(id);
            if (m != null)
            {
                return Ok(m);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]MedioPago m)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Insert(m))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]MedioPago m)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(m))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]MedioPago m)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Delete(m))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}