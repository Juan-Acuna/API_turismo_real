using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using API_TurismoReal.KhipuApiClient;
using API_TurismoReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);

        [Authorize(Roles = "1")]
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
            List<Transaccion> t = await cmd.GetAll<Transaccion>();
            if (t.Count > 0)
            {
                return Ok(t);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
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
            Transaccion t = await cmd.Get<Transaccion>(id);
            if (t != null)
            {
                return Ok(t);
            }
            return BadRequest();
        }
        //[Authorize(Roles = "1,5")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Transaccion t)
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
                /* ZONA KHIPU */
                var pago = await KhipuClient.Peticiones.Pay(t.Comentario,"CLP",t.Monto);
                return Ok(pago);

                /**/
                /*if (await cmd.Insert(t))
                {
                    return Ok();
                }
                return BadRequest();*/
            }
            catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Transaccion t)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Update(t))
            {
                return Ok();
            }
            return BadRequest();
        }
        /*[Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Transaccion t)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(t))
            {
                return Ok();
            }
            return BadRequest();
        }*/
    }
}