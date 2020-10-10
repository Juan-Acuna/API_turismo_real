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
    public class ChoferController : ControllerBase
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
            List<Chofer> chofer = await cmd.GetAll<Chofer>();
            List<PersonaChofer> resultado = new List<PersonaChofer>();
            if (chofer.Count > 0)
            {
                foreach (var c in chofer)
                {
                    Persona p = await cmd.Get<Persona>(c.Rut);
                    if (p != null)
                    {
                        resultado.Add(new PersonaChofer { Chofer = c, Persona = p });
                    }
                }
                return Ok(resultado);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,3")]
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
            Chofer c = await cmd.Get<Chofer>(id);
            if (c != null)
            {
                Persona p = await cmd.Get<Persona>(c.Rut);
                if (p != null)
                {
                    return Ok(new PersonaChofer { Chofer = c, Persona = p });
                }
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PersonaChofer creador)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Chofer chofer = creador.Chofer;
            Persona persona = creador.Persona;
            chofer.Rut = persona.Rut;
            if (await cmd.Insert(persona, false))
            {
                if (await cmd.Insert(chofer))
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
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
            Chofer c = await cmd.Get<Chofer>(id);
            Persona p = await cmd.Get<Persona>(c.Rut);

            if (data.persona.nombres != null)
            {
                p.Nombres = data.persona.nombres;
            }
            if (data.persona.apellidos != null)
            {
                p.Apellidos = data.persona.apellidos;
            }
            if (data.persona.email != null)
            {
                p.Email = data.persona.email;
            }
            if (data.persona.telefono != null)
            {
                p.Telefono = data.persona.telefono;
            }
            if (data.persona.direccion != null)
            {
                p.Direccion = data.persona.direccion;
            }
            if (data.persona.comuna != null)
            {
                p.Comuna = data.persona.comuna;
            }
            if (data.persona.region != null)
            {
                p.Region = data.persona.region;
            }
            if (data.persona.id_genero != null)
            {
                p.Id_genero = data.persona.id_genero;
            }
            if (await cmd.Update(p))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Chofer chofer)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(chofer))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}