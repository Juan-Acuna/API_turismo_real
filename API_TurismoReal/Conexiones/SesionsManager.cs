using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public class SesionsManager
    {
        List<Token> Tokens = new List<Token>();
        public static SesionsManager Sesiones = new SesionsManager();
        public int Activas { get { return Tokens.Count; } }
        public int ClientesActivos { get { return ContarClientes(); } }
        private SesionsManager(){ }

        public Token Registrar(Token token)
        {
            if (Registrado(token))
            {
                Eliminar(token);
            }
            Tokens.Add(token);
            return token;
        }
        public Token Eliminar(Token token)
        {
            Token t = Buscar(token);
            Tokens.Remove(t);
            return t;
        }
        public int IndexOf(Token token)
        {
            return Tokens.IndexOf(token);
        }
        public Token Buscar(Token token, bool noRol = true)
        {
            if ((token.id_rol == 0 && !noRol) && token.username == null)
            {
                return null;
            }
            foreach (var t in Tokens)
            {
                if ((token.id_rol == t.id_rol || noRol) && token.username == null)
                {
                    return t;
                }
            }
            return null;
        }
        public bool Registrado(Token token)
        {
            return Buscar(token)!=null;
        }
        private int ContarClientes()
        {
            var c = 0;
            foreach(var t in Tokens)
            {
                if(Buscar(t,false)!=null)
                {
                    c++;
                }
            }
            return c;
        }
    }
}
