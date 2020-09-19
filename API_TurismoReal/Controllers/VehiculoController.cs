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
    public class VehiculoController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);

        [Authorize(Roles = "1,3")]
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
            List<Vehiculo> v = await cmd.GetAll<Vehiculo>();
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
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            Vehiculo v = await cmd.Get<Vehiculo>(id);
            if (v != null)
            {
                return Ok(v);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Vehiculo v)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Insert(v))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Vehiculo v)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(v))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Vehiculo v)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
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