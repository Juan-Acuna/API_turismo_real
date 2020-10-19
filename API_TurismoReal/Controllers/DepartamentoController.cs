using System;
using System.Collections.Generic;
using System.IO;
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
    public class DepartamentoController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        
        [ProducesResponseType(typeof(List<Departamento>), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
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
                List<Departamento> depto = await cmd.GetAll<Departamento>();
                if (depto.Count > 0)
                {
                    return Ok(depto);
                }
                return BadRequest(MensajeError.Nuevo("No se encontraron departamentos."));
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        
        [ProducesResponseType(typeof(List<Departamento>), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
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

                Departamento d = await cmd.Get<Departamento>(id);
                if (d != null)
                {
                    return Ok(d);
                }
                return BadRequest(MensajeError.Nuevo("No se encontró el departamento"));
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(Departamento), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Departamento depto)
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
                if (await cmd.Insert(depto))
                {
                    var l = await cmd.Get<Localidad>(depto.Id_depto);
                    if (!Directory.Exists(Temp.RUTA_RAIZ + "img\\" + Tools.ToUrlCompatible(l.Nombre) + "\\" + Tools.ToUrlCompatible(depto.Nombre) + "\\"))
                    {
                        Directory.CreateDirectory(Temp.RUTA_RAIZ + "img\\" + Tools.ToUrlCompatible(l.Nombre) + "\\" + Tools.ToUrlCompatible(depto.Nombre));
                    }
                    return Ok((await cmd.Find<Departamento>("Nombre",depto.Nombre))[0]);
                }
                return BadRequest(MensajeError.Nuevo("no se pudo insertar."));
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(Departamento), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute]int id, [FromBody]dynamic data)
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
                Departamento d = await cmd.Get<Departamento>(id);
                if (data.nombre != null)
                {
                    d.Nombre = data.nombre;
                }
                if (data.direccion != null)
                {
                    d.Direccion = data.direccion;
                }
                if (data.arriendo != null)
                {
                    d.Arriendo = data.arriendo;
                }
                if (data.habitaciones != null)
                {
                    d.Habitaciones = data.habitaciones;
                }
                if (data.nombre != null)
                {
                    d.Nombre = data.nombre;
                }
                if (data.nombre != null)
                {
                    d.Nombre = data.nombre;
                }
                if (await cmd.Update(d))
                {
                    return Ok(await cmd.Get<Departamento>(id));
                }
                return BadRequest(MensajeError.Nuevo("No se pudo actualizar."));
            }catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
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
                var d = await cmd.Get<Departamento>(id);
                if (await cmd.Delete(d))
                {
                    return Ok();
                }
                return BadRequest(MensajeError.Nuevo("No se pudo eliminar."));
            }catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
    }
}