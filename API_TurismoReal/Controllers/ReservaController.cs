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
        [HttpGet("usuario/{username}")]
        public async Task<IActionResult> Usuario([FromRoute]String username)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            var s = await cmd.Find<Reserva>("Username", username);
            if (s.Count > 0)
            {
                return Ok(s);
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
        [Authorize(Roles = "1,2,5")]
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
            r.Multa_pagado = 0;
            r.Multa_total = 0;
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
        [Authorize(Roles = "1,2,4,5")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]dynamic data)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Reserva r = new Reserva();
            if (data.Valor_total != null)
            {
                r.Valor_total = data.Valor_total;
            }
            if (data.Valor_pagado != null)
            {
                r.Valor_pagado = data.Valor_pagado;
            }
            if (data.Inicio_estadia != null)
            {
                r.Inicio_estadia = data.Inicio_estadia;
            }
            if (data.Fin_estadia != null)
            {
                r.Fin_estadia = data.Fin_estadia;
            }
            if (data.Desc_checkin != null)
            {
                r.Desc_checkin = data.Desc_checkin;
            }
            if (data.Desc_checkout != null)
            {
                r.Desc_checkout = data.Desc_checkout;
            }
            if (data.Multa_total != null)
            {
                r.Multa_total = data.Multa_total;
            }
            if (data.Multa_pagado != null)
            {
                r.Multa_pagado = data.Multa_pagado;
            }
            if (data.Id_estado != null)
            {
                r.Id_estado = data.Id_estado;
            }
            if (await cmd.Update(r))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "2,5")]
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
            var r = await cmd.Get<Reserva>(id);
            if (await cmd.Delete(r))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}