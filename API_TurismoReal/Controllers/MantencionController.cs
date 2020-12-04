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
        [Authorize(Roles = "1,4")]
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
        [Authorize(Roles = "1,4")]
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
        [Authorize(Roles = "1,4")]
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
        [Authorize(Roles ="1,4")]
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
        [Authorize(Roles = "1,4")]
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
                if (await cmd.Insert(m))
                {
                    var d = await cmd.Get<Departamento>(m.Id_depto);
                    Notificacion n = new Notificacion();
                    n.Username = m.Username;
                    n.Fecha = DateTime.Now;
                    n.Titulo = "Nueva mantención asignada";
                    n.Contenido = "Se ha asignado una mantención al departamento \"" + d.Nombre + "\"(ID:" + d.Id_depto + ") para el día <b>" + m.Fecha.ToShortDateString() + "</b>."
                        + "\n\nLa mantención debe ser llevada a cabo a la brevedad.";
                    n.Visto = '0';
                    n.Link = "http://localhost/agencia/vistas/gestion/index.php#vermantencion";
                    await cmd.Insert(n);
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
        [HttpPatch("{id}/{depto}")]
        public async Task<IActionResult> Patch([FromRoute]int id, [FromRoute]int depto)
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
                m.Hecho = '1';
                if (await cmd.Update(m))
                {
                    var d = await cmd.Get<Departamento>(depto);
                    d.Id_estado = 2;
                    await cmd.Update(d);
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
        [HttpPatch("inhabitable/{id}/{depto}")]
        public async Task<IActionResult> PatchError([FromRoute]int id, [FromRoute]int depto)
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
                m.Hecho = '1';
                if (await cmd.Update(m))
                {
                    var d = await cmd.Get<Departamento>(depto);
                    d.Id_estado = 5;
                    await cmd.Update(d);
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