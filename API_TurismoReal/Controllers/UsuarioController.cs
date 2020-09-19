using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Conection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace API_TurismoReal.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsuarioController : ControllerBase
    {
        OracleCommandManager cmd = new OracleCommandManager(ConexionOracle.Conexion);
        [HttpPost("autenticar")]
        public async Task<IActionResult> Auth([FromBody] Usuario usuario)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            var p = new Procedimiento(ConexionOracle.Conexion, "SP_AUTENTICAR");
            int resultado = 3;
            String msg = "";
            p.Parametros.Add("usr", OracleDbType.Varchar2, usuario.Username, ParameterDirection.Input);
            p.Parametros.Add("pwd", OracleDbType.Varchar2, Tools.Encriptar(usuario.Clave), ParameterDirection.Input);
            p.Parametros.Add("rol", OracleDbType.Int32, ParameterDirection.Output);
            p.Parametros.Add("resultado", OracleDbType.Int32, resultado, ParameterDirection.Output);
            p.Parametros.Add("msg", OracleDbType.Varchar2, msg, ParameterDirection.Output);
            try
            {
                await p.Ejecutar();
                resultado = Convert.ToInt32((decimal)(OracleDecimal)(p.Parametros["resultado"].Value));
                switch (resultado)
                {
                    case 0://Usuario no existe.
                        return BadRequest("Usuario no existe.");
                    case 1://Contraseña incorrecta.
                        return BadRequest("Contraseña incorrecta.");
                    case 2://Inicio exitoso.
                        usuario.Id_rol = Convert.ToInt32((decimal)(OracleDecimal)(p.Parametros["rol"].Value));
                        return Ok(Tools.GenerarToken(usuario));
                    default:
                        return StatusCode(502);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new { error=e,msg=p.Parametros["msg"].Value.ToString()});
            }
            //return StatusCode(418);
        }
        [Authorize(Roles ="1")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            List<Usuario> usuarios = await cmd.GetAll<Usuario>();
            List<PersonaUsuario> resultado = new List<PersonaUsuario>();
            if (usuarios.Count>0)
            {
                foreach(var u in usuarios)
                {
                    Persona p = await cmd.Get<Persona>(u.Rut);
                    if (p != null)
                    {
                        resultado.Add(new PersonaUsuario { Usuario=u,Persona=p });
                    }
                }
                return Ok(resultado);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> Get([FromRoute]String username)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            Usuario u = await cmd.Get<Usuario>(username);
            if ( u != null)
            {
                Persona p = await cmd.Get<Persona>(u.Rut);
                if(p != null)
                {
                    return Ok(new PersonaUsuario { Usuario = u, Persona = p });
                }
            }
            return BadRequest();
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PersonaUsuario creador)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            Usuario usuario = creador.Usuario;
            Persona persona = creador.Persona;
            usuario.Activo = '1';
            usuario.Frecuente = '0';
            usuario.Rut = persona.Rut;
            usuario.Clave = Tools.Encriptar(usuario.Clave);
            if (await cmd.Insert(persona,false))
            {
                if (await cmd.Insert(usuario, false))
                {
                    return Ok(Tools.GenerarToken(usuario));
                }
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]Usuario usuario)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Update(usuario))
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]Usuario usuario)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Delete(usuario))
            {
                return Ok();
            }
            return BadRequest();
        }
        /*[HttpDelete]
        public async Task<IActionResult> Desactivar([FromBody]Usuario usuario)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, "No se pudo establecer comunicacion con la base de datos");
                }
            }
            if (await cmd.Delete(usuario))
            {
                return Ok();
            }
            return BadRequest();
        }*/
    }
}