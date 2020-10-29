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
            String n = "";
            foreach (var l in persona.Nombres.Split(' '))
            {
                n += " " + Tools.Capitalize(l);
            }
            n = n.TrimStart();
            persona.Nombres = n;
            n = "";
            foreach (var l in persona.Apellidos.Split(' '))
            {
                n += " " + Tools.Capitalize(l);
            }
            n = n.TrimStart();
            persona.Apellidos = n;
            n = "";
            foreach (var l in persona.Direccion.Split(' '))
            {
                n += " " + Tools.Capitalize(l);
            }
            n = n.TrimStart();
            persona.Direccion = n;
            persona.Email = persona.Email.ToLower();
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

            if (data.Persona.Nombres != null)
            {
                String n = "";
                foreach (var l in data.Persona.Nombres.Split(' '))
                {
                    n += " " + Tools.Capitalize(l);
                }
                n = n.TrimStart();
                p.Nombres = n;
            }
            if (data.Persona.Apellidos != null)
            {
                String n = "";
                foreach (var l in data.Persona.Apellidos.Split(' '))
                {
                    n += " " + Tools.Capitalize(l);
                }
                n = n.TrimStart();
                p.Apellidos = n;
            }
            if (data.Persona.Email != null)
            {
                p.Email = data.Persona.Email.ToLower();
            }
            if (data.Persona.Telefono != null)
            {
                p.Telefono = data.Persona.Telefono;
            }
            if (data.Persona.Direccion != null)
            {
                String n = "";
                foreach (var l in data.Persona.Direccion.Split(' '))
                {
                    n += " " + Tools.Capitalize(l);
                }
                n = n.TrimStart();
                p.Direccion = n;
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