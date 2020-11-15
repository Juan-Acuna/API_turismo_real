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
        public int Activas { get { return ValidarPorExpiracion(); } }
        public int AdminActivos { get { return ContarPorRol(1); } }
        public int GestResActivos { get { return ContarPorRol(2); } }
        public int GestServActivos { get { return ContarPorRol(3); } }
        public int FuncActivos { get { return ContarPorRol(4); } }
        public int ClientesActivos { get { return ContarPorRol(5); } }
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
            ValidarPorExpiracion();
            return Tokens.IndexOf(token);
        }
        public Token Buscar(Token token, bool noRol = true)
        {
            ValidarPorExpiracion();
            if ((token.id_rol == 0 && !noRol) && token.username == null)
            {
                return null;
            }
            foreach (var t in Tokens)
            {
                if ((token.id_rol == t.id_rol || noRol) && token.username.Equals(t.username))
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
        private int ContarPorRol(int r)
        {
            ValidarPorExpiracion();
            var c = 0;
            foreach(var t in Tokens)
            {
                
                if(t.id_rol==r)
                {
                    c++;
                }
            }
            return c;
        }
        private int ValidarPorExpiracion()
        {
            foreach(var t in Tokens)
            {
                if (DateTime.Compare(t.expiration, DateTime.Now) <= 0)
                {
                    Eliminar(t);
                }
            }
            return Tokens.Count;
        }
    }
}
