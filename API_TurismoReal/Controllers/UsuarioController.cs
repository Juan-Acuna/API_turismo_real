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
        public async Task<IActionResult> Auth([FromHeader(Name = "User-Agent")]String userAgent, [FromBody] Usuario usuario)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
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
                        return BadRequest(new { error="Usuario no existe." });
                    case 1://Contraseña incorrecta.
                        return BadRequest(new { error = "Contraseña incorrecta." });
                    case 2://Inicio exitoso.
                        usuario = await cmd.Get<Usuario>(usuario.Username);
                        Persona p = await cmd.Get<Persona>(usuario.Rut);
                        if (userAgent.Equals("TurismoRealDesktop"))
                        {
                            if (usuario.Id_rol > 1)
                            {
                                return StatusCode(401, new { Error="Acceso Denegado."});
                            }
                        }
                        return Ok(Tools.GenerarToken(usuario,p));
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
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            List<Usuario> usuarios = await cmd.GetAll<Usuario>();
            List<PersonaUsuario> resultado = new List<PersonaUsuario>();
            if (usuarios.Count>0)
            {
                foreach(var u in usuarios)
                {
                    u.Clave = "";
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
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Usuario u = await cmd.Get<Usuario>(username);
            if ( u != null)
            {
                u.Clave = "";
                Persona p = await cmd.Get<Persona>(u.Rut);
                if(p != null)
                {
                    return Ok(new PersonaUsuario { Usuario = u, Persona = p });
                }
            }
            return BadRequest();
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PersonaUsuario creador,[FromHeader(Name = "User-Agent")]String userAgent)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Usuario usuario = creador.Usuario;
            Persona persona = creador.Persona;
            if (!userAgent.Equals("TurismoRealDesktop"))
            {
                usuario.Id_rol = 5;
            }
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
        [HttpPatch("{username}")]
        public async Task<IActionResult> Patch([FromRoute]String username, [FromBody]dynamic data)
        {
            if (!ConexionOracle.Activa)
            {
                ConexionOracle.Open();
                if (!ConexionOracle.Activa)
                {
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            Usuario u = await cmd.Get<Usuario>(username);
            Persona p = await cmd.Get<Persona>(u.Rut);
            if (data.Usuario.Clave != null)
            {
                u.Clave = Tools.Encriptar(data.Usuario.Clave);
            }
            if (data.Usuario.Id_rol != null)
            {
                u.Id_rol = data.Usuario.Id_rol;
            }
            if (data.Usuario.Activo != null)
            {
                u.Activo = data.Usuario.Activo;
            }
            if (data.Usuario.Frecuente != null)
            {
                u.Frecuente = data.Usuario.Frecuente;
            }
            if (data.Persona.Nombres != null)
            {
                p.Nombres = data.Persona.Nombres;
            }
            if (data.Persona.Apellidos != null)
            {
                p.Apellidos = data.Persona.Apellidos;
            }
            if (data.Persona.Email != null)
            {
                p.Email = data.Persona.Email;
            }
            if (data.Persona.Telefono != null)
            {
                p.Telefono = data.Persona.Telefono;
            }
            if (data.Persona.Direccion != null)
            {
                p.Direccion = data.Persona.Direccion;
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
            if (await cmd.Update(u))
            {
                if(await cmd.Update(p))
                {
                    return Ok();
                }
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
                    return StatusCode(504, ConexionOracle.NoConResponse);
                }
            }
            if (await cmd.Delete(usuario))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}