using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public class Procedimiento
    {
        public OracleConnection Conexion { get; set; }
        public String Nombre { get; set; }
        OracleCommand cmd = new OracleCommand();
        public OracleParameterCollection Parametros { get;set; }
        public Procedimiento()
        {
            Parametros = cmd.Parameters;
        }
        public Procedimiento(String nombre)
        {
            Parametros = cmd.Parameters;
            Nombre = nombre;
        }
        public Procedimiento(OracleConnection con)
        {
            Parametros = cmd.Parameters;
            Conexion = con;
        }
        public Procedimiento(OracleConnection con, String nombre)
        {
            Parametros = cmd.Parameters;
            Conexion = con;
            Nombre = nombre;
        }
        public async Task Ejecutar()
        {
            cmd.Connection = Conexion;
            cmd.CommandText = Nombre;
            cmd.CommandType = CommandType.StoredProcedure;
            /*foreach(var p in Parametros)
            {
                cmd.Parameters.Add(p);
            }*/
            await cmd.ExecuteNonQueryAsync();
        }
    }
    public class InsertarTabla
    {
        public OracleConnection Conexion { get; set; }
        private OracleParameterCollection Parametros { get; set; }
        public InsertarTabla() { }
        public InsertarTabla(OracleConnection con)
        {
            Conexion = con;
        }
        public async Task Insertar()
        {
            var cmd = new OracleCommand();
            cmd.Connection = Conexion;
            cmd.CommandText = "SP_INSERTAR";
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var p in Parametros)
            {
                cmd.Parameters.Add(p);
            }
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
