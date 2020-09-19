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
    public class TipoMultaController : ControllerBase
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
            List<TipoMulta> t = await cmd.GetAll<TipoMulta>();
            if (t.Count > 0)
            {
                return Ok(t);
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
            TipoMulta t = await cmd.Get<TipoMulta>(id);
            if (t != null)
            {
                return Ok(t);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TipoMulta t)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Insert(t))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]TipoMulta t)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(t))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]TipoMulta t)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Delete(t))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}