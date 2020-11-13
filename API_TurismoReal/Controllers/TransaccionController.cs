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
//using Transbank.Webpay;

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
        /*
        [HttpPost("notificar/{token}")]
        public async Task<IActionResult> Notificar([FromRoute]String token)
        {
            try
            {
                var tl = await cmd.Find<Transaccion>("Token",token);
                if (tl.Count <= 0)
                {
                    return BadRequest(MensajeError.Nuevo("Transaccion no existe o medio pago incorrecto"));
                }
                Transaccion tr = tl[0];
                var trs = new Webpay(Transbank.Webpay.Configuration.ForTestingWebpayPlusNormal()).NormalTransaction;
                var response = trs.getTransactionResult(tr.Token);
                var output = response.detailOutput[0];
                if (output.responseCode == 0)
                {
                    tr.Listo = '1';
                    Reserva r = await cmd.Get<Reserva>(tr.Id_reserva);
                    r.Valor_pagado += Int32.Parse(tr.Monto.ToString());
                    if (r.Valor_pagado == r.Valor_total)
                    {
                        r.Id_estado = 3;
                    }
                    else if (r.Valor_pagado == Math.Round((double)(r.Valor_total / 2), 0, MidpointRounding.AwayFromZero))
                    {
                        r.Id_estado = 2;
                    }
                    else
                    {
                        r.Id_estado = 1;
                    }
                    await cmd.Update(r);
                    var list = await cmd.Find<Reserva>("Username", tr.Username);
                    List<Reserva> rl = new List<Reserva>();
                    foreach (var res in list)
                    {
                        if (res.Id_estado == 3 || res.Id_estado == 4)
                        {
                            rl.Add(res);
                        }
                    }
                    if (rl.Count > 9)
                    {
                        var u = await cmd.Get<Usuario>(tr.Username);
                        u.Frecuente = '1';
                        await cmd.Update(u);
                    }
                    object carga = new {
                        url = response.urlRedirection,
                        codigo = output.responseCode,
                        monto = output.amount,
                        trans = tr,
                        auth = output.authorizationCode
                    };
                    return Ok(carga);
                }
                return BadRequest(new { codigo = output.responseCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, MensajeError.Nuevo(ex.Message));
            }
        }*/

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
                t.Id_pago = trId;
                t.Fecha = DateTime.Now;
                if (t.Id_medio == 1)
                {
                    /* ZONA KHIPU */
                    Khipu.Client.Configuration.ReceiverId = Secret.T_RESEIVER_ID;
                    Khipu.Client.Configuration.Secret = Secret.T_SECRET_KEY;
                    PaymentsApi pago = new PaymentsApi();
                    PaymentsCreateResponse response = pago.PaymentsPost(t.Comentario, "CLP", t.Monto, transactionId: trId,
                        returnUrl: ServerURLs.PagarUrl(Acccion.repay, t.Id_reserva, trId),
                        cancelUrl: ServerURLs.PagarUrl(Acccion.cancel, t.Id_reserva, trId, t.Monto));
                    /**/
                    t.Listo = '1';
                    var r = await cmd.Get<Reserva>(t.Id_reserva);
                    if (r == null)
                    {
                        return BadRequest(MensajeError.Nuevo("Reserva no existe."));
                    }
                    r.Valor_pagado += Convert.ToInt32(t.Monto);
                    r.n_pagos = r.n_pagos + 1;
                    if (r.Valor_pagado == r.Valor_total)
                    {
                        r.Id_estado = 3;
                    }
                    else if (r.Valor_pagado == Math.Round((double)(r.Valor_total / 2), 0, MidpointRounding.AwayFromZero))
                    {
                        r.Id_estado = 2;
                    }
                    else
                    {
                        r.Id_estado = 1;
                    }
                    await cmd.Update(r);
                    var list = await cmd.Find<Reserva>("Username", t.Username);
                    List<Reserva> rl = new List<Reserva>();
                    foreach (var res in list)
                    {
                        if (res.Id_estado == 3 || res.Id_estado == 4)
                        {
                            rl.Add(res);
                        }
                    }
                    if (rl.Count > 9)
                    {
                        var u = await cmd.Get<Usuario>(t.Username);
                        u.Frecuente = '1';
                        await cmd.Update(u);
                    }
                    if (await cmd.Insert(t, false))
                    {
                        return Ok(response);
                    }
                }
                else
                {/*
                    var trs = new Webpay(Transbank.Webpay.Configuration.ForTestingWebpayPlusNormal()).NormalTransaction;
                    var initResult = trs.initTransaction(t.Monto,trId,"sesion-"+trId, ServerURLs.PagarUrl(Acccion.pay, t.Id_reserva, trId, t.Monto), ServerURLs.PagarUrl(Acccion.commit, t.Id_reserva, trId, t.Monto));
                    var token = initResult.token;
                    t.Token = token;
                    var url = initResult.url;
                    if (await cmd.Insert(t, false))
                    {
                        return Ok(new { Token = token, Url = url });
                    }*/
                }
                return BadRequest();
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