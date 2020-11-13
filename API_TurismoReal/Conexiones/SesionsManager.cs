﻿using System;
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
        public Token Buscar(Token token)
        {
            if (token.username == null)
            {
                return null;
            }
            foreach (var t in Tokens)
            {
                if (token.username.Equals(t.username))
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
    }
}