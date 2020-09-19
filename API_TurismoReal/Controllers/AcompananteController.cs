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
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
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
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
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
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
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
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Acompanante acom)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(acom))
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
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
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