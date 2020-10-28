using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using API_TurismoReal.Models;
using Khipu.Api;
using Khipu.Client;
using Khipu.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace API_TurismoReal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);

        [HttpGet("status/{id}")]
        public async Task<IActionResult> GetStatus([FromRoute]String id)
        {
            var t = await cmd.Get<Transaccion>(id);
            return Ok(new { Listo = t.Listo == '1' });
        }

        [HttpPost("notificar")]
        public async Task<IActionResult> Notificar([FromBody]String apiVersion, [FromBody]String notificationToken)
        {
            if (apiVersion.Equals("1.3"))
            {
                Transaccion tr;
                PaymentsApi p = new PaymentsApi();
                try
                {
                    PaymentsResponse response = p.PaymentsGet(notificationToken);
                    if (response.ReceiverId.Equals(Secret.T_RESEIVER_ID))
                    {
                        tr = await cmd.Get<Transaccion>(response.TransactionId);
                        if(response.Status.Equals("done") && response.Amount == tr.Monto)
                        {
                            tr.Listo = '1';
                            tr.Token = response.NotificationToken;
                            Reserva r = await cmd.Get<Reserva>(tr.Id_reserva);
                            r.Valor_pagado += tr.Monto;
                            if (r.Valor_pagado == r.Valor_total)
                            {
                                r.Id_estado = 3;
                            }
                            else
                            {
                                r.Id_estado = 2;
                            }
                            await cmd.Update(r);
                            var list = await cmd.Find<Reserva>("Username", tr.Username);
                            List<Reserva> rl = new List<Reserva>();
                            foreach(var res in list)
                            {
                                if(res.Id_estado==3 || res.Id_estado == 4)
                                {
                                    rl.Add(res);
                                }
                            }
                            if (rl.Count>9)
                            {
                                var u = await cmd.Get<Usuario>(tr.Username);
                                u.Frecuente = '1';
                                await cmd.Update(u);
                            }
                        }
                    }
                }catch(Exception ex)
                {
                    return StatusCode(500, MensajeError.Nuevo(ex.Message));
                }
            }
            return Ok();
        }

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
                Procedimiento p = new Procedimiento(ConexionOracle.Conexion, "SP_ID_PAGO");
                p.Parametros.Add("id_pago", OracleDbType.Int32, ParameterDirection.Output);
                await p.Ejecutar();
                int idf = Convert.ToInt32((decimal)(OracleDecimal)(p.Parametros["id_pago"].Value));
                String trId = "TTR" + idf.ToString();
                /* ZONA KHIPU */
                Configuration.ReceiverId = Secret.T_RESEIVER_ID;
                Configuration.Secret = Secret.T_SECRET_KEY;
                PaymentsApi pago = new PaymentsApi();
                PaymentsCreateResponse response = pago.PaymentsPost(t.Comentario, "CLP", t.Monto,transactionId:trId);
                /**/
                t.Id_pago = trId;
                t.Fecha = DateTime.Now;
                t.Id_medio = 1;
                t.Listo = '0';

                if (await cmd.Insert(t,false))
                {
                    return Ok();
                }
                return Ok(response);
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