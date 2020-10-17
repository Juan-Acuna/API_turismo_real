using API_TurismoReal.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public static class Tools
    {
        public static DateTime StringToDate(String date, DateFormat format = DateFormat.DayMonthYear)
        {
            String d = "";
            String y = "";
            String m = "";
            date = date.Replace("/", "");
            date = date.Replace("-", "");
            switch (format)
            {
                case DateFormat.DayMonthYear:
                    d = date.Substring(0, 2);
                    m = date.Substring(2, 2);
                    y = date.Substring(4, 4);
                    break;
                case DateFormat.MonthDayYear:
                    m = date.Substring(0, 2);
                    d = date.Substring(2, 2);
                    y = date.Substring(4, 4);
                    break;
                case DateFormat.YearMonthDay:
                    y = date.Substring(0, 4);
                    m = date.Substring(4, 2);
                    d = date.Substring(6, 2);
                    break;
            }
            return new DateTime(Int32.Parse(y), Int32.Parse(m), Int32.Parse(d));
        }
        public static String DateToString(DateTime date, DateFormat outputFormat, char divisor = '/')
        {
            String output = "";
            String d;
            String y;
            String m;
            if (date.Day < 10)
            {
                d = "0" + date.Day.ToString();
            }
            else
            {
                d = date.Day.ToString();
            }
            if (date.Month < 10)
            {
                m = "0" + date.Month.ToString();
            }
            else
            {
                m = date.Month.ToString();
            }
            y = date.Year.ToString();
            switch (outputFormat)
            {
                case DateFormat.DayMonthYear:
                    output = d + divisor + m + divisor + y;
                    break;
                case DateFormat.MonthDayYear:
                    output = m + divisor + d + divisor + y;
                    break;
                case DateFormat.YearMonthDay:
                    output = y + divisor + m + divisor + d;
                    break;
            }
            return output;
        }
        public static String Capitalize(String str)
        {
            String s = "";
            foreach (var l in str)
            {
                if (l >= 65 && l <= 90)
                {
                    s += ((char)(l + 32)).ToString();
                }
                else
                {
                    s += l.ToString();
                }
            }
            s = ((char)(s[0] - 32)).ToString() + s.Remove(0, 1);
            return s;
        }
        public static Token GenerarToken(Usuario usuario, Persona persona)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,usuario.Id_rol.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("kdljsfksdda234jf654dkgfHGDjsfkFglDSAGFshgfdHgdHGdfjhgfjT$#tsj&iJStrhfhk"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: "turismoreal.cl",
               audience: "Turistas",
               claims: claims,
               expires: expiration,
               signingCredentials: creds);
            return new Token(new JwtSecurityTokenHandler().WriteToken(token), expiration, usuario.Username, persona.Nombres, persona.Apellidos);
        }

        public static string Encriptar(String texto)
        {
            return new ASCIIEncoding()
                .GetString(new SHA1CryptoServiceProvider()
                .ComputeHash(Encoding.ASCII.GetBytes(texto)));
        }
        public static String CodigoAleatorio(String sal, int largo = 10)
        {
            String clave = "";
            int suma = DateTime.Now.Millisecond;
            foreach (var c in sal)
            {
                suma += c;
            }
            var r = new Random(suma);
            for (int i = 0; i < largo; i++)
            {
                int l = 65 + r.Next(57);
                if (l >= 91 && l <= 96)
                {
                    l = 95;
                }
                clave += (char)l;
            }
            return clave;
        }
        public static String EncriptarUrlCompatible(String texto)
        {
            var encoding = new ASCIIEncoding();
            var s = encoding.GetString(new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(texto)));
            String r = "";
            foreach (var c in s)
            {
                if(c == 45 || (c >=65 && c <= 90) || c == 95 || (c >= 97 && c <= 122))
                {
                    r += c;
                }
                else
                {
                    r += "0";
                }
            }
            return r;
        }
        public static String ToUrlCompatible(String texto)
        {
            String r = "";
            foreach (var c in texto)
            {
                if (c == 45 || c == 46 || (c >= 65 && c <= 90) || c == 95 || (c >= 97 && c <= 122))
                {
                    r += c;
                }
                else if(c == 32)
                {
                    r += '-';
                }
                else if(c == 'ñ')
                {
                    r += 'n';
                }
                else if (c == 'Ñ')
                {
                    r += 'N';
                }
                else
                {
                    r += '_';
                }
            }
            return r;
        }
    }
}
