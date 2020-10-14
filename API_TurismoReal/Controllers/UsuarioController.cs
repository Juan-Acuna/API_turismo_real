using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using API_TurismoReal.Conexiones;
using API_TurismoReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace API_TurismoReal.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsuarioController : ControllerBase
    {
        MensajesEstandar Mensajes = MensajesEstandar.Instancia;
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
                        Persona persona = await cmd.Get<Persona>(usuario.Rut);
                        if (userAgent.Equals("TurismoRealDesktop"))
                        {
                            if (usuario.Id_rol > 1)
                            {
                                return StatusCode(401, new { Error="Acceso Denegado."});
                            }
                        }
                        return Ok(Tools.GenerarToken(usuario,persona));
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
            try
            {
                Usuario usuario = creador.Usuario;
                Persona persona = creador.Persona;
                if (!userAgent.Equals("TurismoRealDesktop"))
                {
                    usuario.Id_rol = 5;
                    usuario.Clave = Tools.Encriptar(usuario.Clave);
                }
                else
                {
                    usuario.Clave = Tools.Encriptar(Tools.CodigoAleatorio(persona.Rut));
                }
                usuario.Activo = '1';
                usuario.Frecuente = '0';
                usuario.Rut = persona.Rut;
                if (userAgent.Equals("TurismoRealDesktop"))
                {
                    usuario.Activo = '0';
                    usuario.Clave = Tools.Encriptar(Tools.CodigoAleatorio(usuario.Username));
                    var reset = new ClaveReset
                    {
                        Codigo = Tools.CodigoAleatorio(persona.Rut),
                        Fecha = DateTime.Now,
                        Vencimiento = DateTime.Now.AddMonths(1),
                        Canjeado = '0',
                        Username = usuario.Username
                    };
                    if (await cmd.Insert(reset))
                    {
                        if (await cmd.Insert(persona, false))
                        {
                            if (await cmd.Insert(usuario, false))
                            {
                                var rol = await cmd.Get<Rol>(usuario.Id_rol);
                                String salt = Tools.Encriptar(usuario.Username + reset.Codigo);
                                var m = Mensajes.ActivacionCuenta;
                                m.AgregarDestinatario(persona.Email, persona.Nombres + " " + persona.Apellidos);
                                m.ConfigurarAsunto("rol", rol.Nombre);
                                m.ConfigurarMensaje("rol", rol.Nombre);
                                m.ConfigurarMensaje("usuario", usuario.Username);
                                m.ConfigurarMensaje("codigo", reset.Codigo);
                                m.ConfigurarMensaje("salt", salt);
                                ClienteSmtp.Enviar(m);
                                return Ok();
                            }
                            await cmd.Delete(usuario);
                        }
                        await cmd.Delete(persona);
                    }
                }
                else if (await cmd.Insert(persona, false))
                {
                    if (await cmd.Insert(usuario, false))
                    {
                        return Ok(Tools.GenerarToken(usuario, persona));
                    }
                    await cmd.Delete(persona);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                var a = new
                {
                    Error= new
                    {
                        Mensaje = e.Message,
                        Inner = e.InnerException,
                        Fuente = e.Source
                    }
                };
                return StatusCode(400, a);
            }
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
            if (data.Usuario.Clave != null || ((String)data.Usuario.Clave).Trim().Length>0)
            {
                u.Clave = Tools.Encriptar((String)data.Usuario.Clave);
            }
            if (data.Usuario.Id_rol != null)
            {
                u.Id_rol = data.Usuario.Id_rol;
            }
            if (data.Usuario.Activo != null)
            {
                u.Activo = data.Usuario.Activo;
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