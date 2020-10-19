using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using API_TurismoReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcompananteController : ControllerBase
    {

        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [Authorize(Roles = "1,5")]
        [ProducesResponseType(typeof(List<PersonaAcompanante>),200)]
        [ProducesResponseType(typeof(MensajeError),400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError),504)]
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
                List<Acompanante> acom = await cmd.GetAll<Acompanante>();
                List<PersonaAcompanante> resultado = new List<PersonaAcompanante>();
                if (acom.Count > 0)
                {
                    foreach (var a in acom)
                    {
                        Persona p = await cmd.Get<Persona>(a.Rut);
                        if (p != null)
                        {
                            resultado.Add(new PersonaAcompanante { Acompanante = a, Persona = p });
                        }
                    }
                    return Ok(resultado);
                }
                return BadRequest(MensajeError.Nuevo("No se encontraron acompañantes."));
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1,5")]
        [ProducesResponseType(typeof(PersonaAcompanante), 200)]
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
                Acompanante a = await cmd.Get<Acompanante>(id);
                if (a != null)
                {
                    Persona p = await cmd.Get<Persona>(a.Rut);
                    if (p != null)
                    {
                        return Ok(new PersonaAcompanante { Acompanante = a, Persona = p });
                    }
                }
            return BadRequest(MensajeError.Nuevo("No se encontró al acompañante."));
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
            
        }
        [Authorize(Roles = "1,5")]
        [ProducesResponseType(typeof(PersonaAcompanante), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PersonaAcompanante creador)
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
                Acompanante acom = creador.Acompanante;
                Persona persona = creador.Persona;
                acom.Rut = persona.Rut;
                if (await cmd.Insert(persona, false))
                {
                    if (await cmd.Insert(acom))
                    {
                        var p = await cmd.Get<Persona>(persona.Rut);
                        var a = (await cmd.Find<Acompanante>("Rut", p.Rut))[0];
                        return Ok(new PersonaAcompanante { Acompanante = a, Persona = p });
                    }
                    await cmd.Delete(persona);
                }
                return BadRequest(MensajeError.Nuevo("No se pudo insertar."));
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1,5")]
        [ProducesResponseType(typeof(PersonaAcompanante), 200)]
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
                Acompanante a = await cmd.Get<Acompanante>(id);
                Persona p = await cmd.Get<Persona>(a.Rut);
                if (data.Persona.Nombres != null)
                {
                    p.Nombres = data.Persona.Nombres;
                }
                if (data.Persona.Apellidos != null)
                {
                    p.Apellidos = data.Persona.Apellidos;
                }
                if (data.Persona.Email != null)
                {
                    p.Email = data.Persona.Email;
                }
                if (data.Persona.Telefono != null)
                {
                    p.Telefono = data.Persona.Telefono;
                }
                if (data.Persona.Direccion != null)
                {
                    p.Direccion = data.Persona.Direccion;
                }
                if (data.Persona.Comuna != null)
                {
                    p.Comuna = data.Persona.Comuna;
                }
                if (data.Persona.Region != null)
                {
                    p.Region = data.Persona.Region;
                }
                if (data.Persona.Id_genero != null)
                {
                    p.Id_genero = data.Persona.Id_genero;
                }
                if (await cmd.Update(p))
                {
                    a = await cmd.Get<Acompanante>(id);
                    p = await cmd.Get<Persona>(a.Rut);
                    return Ok(new PersonaAcompanante { Persona = p, Acompanante = a });
                }
                return BadRequest(MensajeError.Nuevo("No se pudo actualizar."));
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1,5")]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Acompanante acom)
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
                if (await cmd.Delete(acom))
                {
                    return Ok();
                }
                return BadRequest(MensajeError.Nuevo("No se pudo eliminar."));
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
    }
}