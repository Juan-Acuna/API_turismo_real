using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcompananteController : ControllerBase
    {

        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [Authorize(Roles = "1,5")]
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
            return BadRequest();
        }
        [Authorize(Roles = "1,5")]
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
            Acompanante a = await cmd.Get<Acompanante>(id);
            if (a != null)
            {
                Persona p = await cmd.Get<Persona>(a.Rut);
                if (p != null)
                {
                    return Ok(new PersonaAcompanante { Acompanante = a, Persona = p });
                }
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,5")]
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
            Acompanante acom = creador.Acompanante;
            Persona persona = creador.Persona;
            acom.Rut = persona.Rut;
            if (await cmd.Insert(persona, false))
            {
                if (await cmd.Insert(acom))
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,5")]
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
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,5")]
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
            if (await cmd.Delete(acom))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}