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
                depto.Id_estado = 1;
                if (await cmd.Insert(depto))
                {
                    var l = await cmd.Get<Localidad>(depto.Id_localidad);
                    if (!Directory.Exists(Secret.RUTA_RAIZ + "img\\" + Tools.ToUrlCompatible(l.Nombre) + "\\" + Tools.ToUrlCompatible(depto.Nombre) + "\\"))
                    {
                        Directory.CreateDirectory(Secret.RUTA_RAIZ + "img\\" + Tools.ToUrlCompatible(l.Nombre) + "\\" + Tools.ToUrlCompatible(depto.Nombre));
                    }
                    return Ok((await cmd.Find<Departamento>("Nombre",depto.Nombre)));
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
                if (data.Nombre != null)
                {
                    d.Nombre = data.Nombre;
                }
                if (data.Direccion != null)
                {
                    d.Direccion = data.Direccion;
                }
                if (data.Arriendo != null)
                {
                    d.Arriendo = data.Arriendo;
                }
                if (data.Habitaciones != null)
                {
                    d.Habitaciones = data.Habitaciones;
                }
                if (data.Id_localidad != null)
                {
                    d.Id_localidad = data.Id_localidad;
                }
                if (data.Mts_cuadrados != null)
                {
                    d.Mts_cuadrados = data.Mts_cuadrados;
                }
                if (data.Banos != null)
                {
                    d.Banos = data.Banos;
                }
                if (data.Dividendo != null)
                {
                    d.Dividendo = data.Dividendo;
                }
                if (data.Id_estado != null)
                {
                    d.Id_estado = data.Id_estado;
                }
                if (data.Contribuciones != null)
                {
                    d.Contribuciones = data.Contribuciones;
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