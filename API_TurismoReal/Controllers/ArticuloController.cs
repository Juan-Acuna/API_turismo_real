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
    public class ArticuloController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [Authorize(Roles = "1,2")]
        [HttpGet("asignado/{id}")]
        public async Task<IActionResult> Asignado([FromRoute]int id)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            List<DeptoArticulo> art = await cmd.Find<DeptoArticulo>("Id_articulo",id);
            if (art.Count > 0)
            {
                var d = await cmd.Get<Departamento>(art[0].Id_depto);
                return Ok(d);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar([FromBody]DeptoArticulo da)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Insert(da,false))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
        [HttpDelete("desasignar/{id}")]
        public async Task<IActionResult> Desasignar([FromRoute]int id)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            var x = await cmd.Find<DeptoArticulo>("Id_articulo",id);
            if (x.Count > 0)
            {
                var da = x[0];
                if (await cmd.Delete(da))
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
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
            List<Articulo> art = await cmd.GetAll<Articulo>();
            if (art.Count > 0)
            {
                return Ok(art);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
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
            Articulo a = await cmd.Get<Articulo>(id);
            if (a != null)
            {
                return Ok(a);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
        [HttpGet("proxy")]
        public async Task<IActionResult> GetProxy()
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            List<Articulo> art = await cmd.GetAll<Articulo>();
            if (art.Count > 0)
            {
                List<ProxyArticulo> lista = new List<ProxyArticulo>();
                foreach(var a in art)
                {
                    var pa = new ProxyArticulo();
                    pa.Articulo = a;
                    pa.Asignado = false;
                    var x = await cmd.Find<DeptoArticulo>("Id_articulo",a.Id_articulo);
                    if (x.Count > 0)
                    {
                        pa.Asignado = true;
                        pa.Depto = x[0].Id_depto;
                    }
                    lista.Add(pa);
                }
                return Ok(lista);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1,2")]
        [HttpGet("proxy/{id}")]
        public async Task<IActionResult> GetProxy([FromRoute]int id)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Articulo a = await cmd.Get<Articulo>(id);
            if (a != null)
            {
                var pa = new ProxyArticulo();
                pa.Articulo = a;
                pa.Asignado = false;
                var x = await cmd.Find<DeptoArticulo>("Id_articulo",id);
                if (x.Count > 0)
                {
                    pa.Depto = x[0].Id_depto;
                }
                return Ok(pa);
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Articulo art)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Insert(art))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Articulo art)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Update(art))
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Articulo art)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(art))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}