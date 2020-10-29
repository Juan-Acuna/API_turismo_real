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
                    return StatusCode(504, ConexionOracle.NoConResponse);
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
        [HttpGet("{patente}")]
        public async Task<IActionResult> Get([FromRoute]String patente)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Vehiculo v = await cmd.Get<Vehiculo>(patente);
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
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            v.Patente = v.Patente.ToUpper();
            if (await cmd.Insert(v))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPatch("{patente}")]
        public async Task<IActionResult> Patch([FromBody]dynamic data,[FromRoute]String patente)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Vehiculo v = await cmd.Get<Vehiculo>(patente.ToUpper());
            if (data.Marca != null)
            {
                v.Marca = data.Marca;
            }
            if (data.Modelo != null)
            {
                v.Modelo = data.Modelo;
            }
            if (await cmd.Update(v))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpDelete("{patente}")]
        public async Task<IActionResult> Delete([FromRoute]String patente)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            var v = await cmd.Get<Vehiculo>(patente.ToUpper());
            if (await cmd.Delete(v))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}