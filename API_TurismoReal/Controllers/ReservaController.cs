using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using API_TurismoReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [Authorize]
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
            List<Reserva> r = await cmd.GetAll<Reserva>();
            if (r.Count > 0)
            {
                return Ok(r);
            }
            return BadRequest();
        }
        [Authorize]
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
            Reserva r = await cmd.Get<Reserva>(id);
            if (r != null)
            {
                return Ok(r);
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Reserva r)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Procedimiento p = new Procedimiento(ConexionOracle.Conexion, "SP_ID_RESERVA");
            p.Parametros.Add("id_reserva", OracleDbType.Int32, ParameterDirection.Output);
            await p.Ejecutar();
            int idf = Convert.ToInt32((decimal)(OracleDecimal)(p.Parametros["id_reserva"].Value));
            r.Id_reserva = idf;
            r.Valor_pagado = 0;
            r.Fecha = DateTime.Now;
            r.Id_estado = 1;
            if (await cmd.Insert(r,false))
            {
                return Ok(r);
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,4,5")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Reserva r)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Update(r))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Reserva r)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(r))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}