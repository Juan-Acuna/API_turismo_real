using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    public class FotoController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);

        [ProducesResponseType(typeof(List<Foto>), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
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
            try
            {
                List<Foto> f = await cmd.GetAll<Foto>();
                if (f.Count > 0)
                {
                    return Ok(f);
                }
                return BadRequest(MensajeError.Nuevo("No se encontraron imagenes."));
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [ProducesResponseType(typeof(List<Foto>), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
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
                var f = await cmd.Find<Foto>("Id_depto", id);
                if (f.Count > 0)
                {
                    return Ok(f);
                }
                return BadRequest(MensajeError.Nuevo("No existen imagenes del departamento."));
            }
            catch (Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(MensajeCorriente), 200)]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpPost("{id_depto}")]
        public async Task<IActionResult> Post([FromForm]List<IFormFile> imagenes, [FromRoute]int id_depto)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Departamento depto = await cmd.Get<Departamento>(id_depto);
            Localidad localidad = await cmd.Get<Localidad>(depto.Id_localidad);
            List<Foto> listaFotos = new List<Foto>();
            Foto f;
            Procedimiento p = new Procedimiento(ConexionOracle.Conexion, "SP_ID_FOTO");
            p.Parametros.Add("id_foto", OracleDbType.Int32, ParameterDirection.Output);//id_depto
            String rutaBase = Secret.RUTA_RAIZ;
            try
            {
                if (imagenes.Count > 0)
                {
                    foreach (var foto in imagenes)
                    {
                        await p.Ejecutar();
                        int idf = Convert.ToInt32((decimal)(OracleDecimal)(p.Parametros["id_foto"].Value));
                        String subruta = "img\\" + Tools.ToUrlCompatible(localidad.Nombre.ToLower()) + "\\" + Tools.ToUrlCompatible(depto.Nombre.ToLower()) + "\\" + Tools.ToUrlCompatible(depto.Nombre.Replace(" ", "_")).ToUpper() + "_" + idf.ToString() + Path.GetExtension(foto.FileName);
                        using (var stream = System.IO.File.Create(rutaBase + subruta))
                        {
                            await foto.CopyToAsync(stream);
                        }
                        f = new Foto
                        {
                            Id_foto = idf,
                            Ruta = ("http://turismoreal.xyz/" + subruta).Replace("\\", "/"),
                            Id_depto = depto.Id_depto
                        };
                        listaFotos.Add(f);
                    }
                    int cont = 0;
                    foreach (var item in listaFotos)
                    {
                        if (await cmd.Insert(item, false))
                        {
                            cont++;
                        }
                    }
                    if (cont == 1)
                    {
                        return Ok(new { Mensaje = "La Imagen fue subida exitosamente." });
                    }
                    else if (cont > 0)
                    {
                        return Ok(new { Mensaje = cont.ToString() + " Imagenes fueron subidas exitosamente." });
                    }
                    else
                    {
                        return BadRequest(new { Error = "No fue posible subir la(s) imagen(es)." });
                    }
                }
                else
                {
                    return BadRequest(new { Error = "No se recibieron imagenes." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromForm]IFormFile imagen,[FromRoute]int id)
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
                Foto f = await cmd.Get<Foto>(id);
                Departamento d = await cmd.Get<Departamento>(f.Id_depto);
                Localidad l = await cmd.Get<Localidad>(d.Id_localidad);
                String rutaBase = Secret.RUTA_RAIZ;
                String subruta = "img\\" + Tools.ToUrlCompatible(l.Nombre.ToLower()) + "\\" + Tools.ToUrlCompatible(d.Nombre.ToLower()) + "\\" + Tools.ToUrlCompatible(d.Nombre.Replace(" ", "_")).ToUpper() + "_" + id.ToString() + Path.GetExtension(imagen.FileName);
                var s = "img\\" + Tools.ToUrlCompatible(l.Nombre.ToLower()) + "\\" + Tools.ToUrlCompatible(d.Nombre.ToLower()) + "\\" + f.Ruta.Split("/").Last();
                if (System.IO.File.Exists(rutaBase + s))
                {
                    System.IO.File.Delete(rutaBase + s);
                }
                using (var stream = System.IO.File.Create(rutaBase + subruta))
                {
                    await imagen.CopyToAsync(stream);
                }
                f.Ruta = ("http://turismoreal.xyz/" + subruta).Replace("\\", "/");
                if(await cmd.Update(f))
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
        [Authorize(Roles = "1")]
        [ProducesResponseType(typeof(MensajeError), 400)]
        [ProducesResponseType(typeof(MensajeError), 500)]
        [ProducesResponseType(typeof(MensajeError), 504)]
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
            try
            {
                Foto f = await cmd.Get<Foto>(id);
                Departamento d = await cmd.Get<Departamento>(f.Id_depto);
                Localidad l = await cmd.Get<Localidad>(d.Id_localidad);
                String ruta = Secret.RUTA_RAIZ + "img\\" + Tools.ToUrlCompatible(l.Nombre.ToLower()) + "\\" + Tools.ToUrlCompatible(d.Nombre.ToLower()) + "\\" + f.Ruta.Split('/').Last().ToUpper();
                if (System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                }
                if (await cmd.Delete(f))
                {
                    return Ok();
                }
                return BadRequest();
            }catch(Exception e)
            {
                return StatusCode(500, MensajeError.Nuevo(e.Message));
            }
        }
    }
}