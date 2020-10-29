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
    public class MantencionController : ControllerBase
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
            try
            {
                List<Mantencion> m = await cmd.GetAll<Mantencion>();
                if (m.Count > 0)
                {
                    return Ok(m);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [HttpGet("depto/{id}")]
        public async Task<IActionResult> Find([FromRoute]int id)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            try
            {
                List<Mantencion> m = await cmd.Find<Mantencion>("Id_depto", id);
                if (m.Count > 0)
                {
                    return Ok(m);
                }
                return BadRequest(MensajeError.Nuevo("El departamento no tiene mantenciones asociadas."));
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [HttpGet("funcionario/{username}")]
        public async Task<IActionResult> FindFuncionario([FromRoute]String username)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            try
            {
                List<Mantencion> m = await cmd.Find<Mantencion>("Username", username);
                if (m.Count > 0)
                {
                    return Ok(m);
                }
                return BadRequest(MensajeError.Nuevo("El Funcionario no tiene mantenciones asociadas."));
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
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
            try
            {
                Mantencion m = await cmd.Get<Mantencion>(id);
                if (m != null)
                {
                    return Ok(m);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Mantencion m)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            try
            {
                //if(m.Username==null || m.Username.Equals(""))
                if (await cmd.Insert(m))
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute]int id,[FromBody]dynamic data)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            try
            {
                var m = await cmd.Get<Mantencion>(id);

                if (await cmd.Update(m))
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Mantencion m)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            try
            {
                if (await cmd.Delete(m))
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
    }
}