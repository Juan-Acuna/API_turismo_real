using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API_TurismoReal.Models;

namespace API_TurismoReal.Conexiones
{
    /// <summary>
    /// Una simple clase en C# que a traves de una conexión OracleConection permite ejecutar un CRUD completo.
    /// </summary>
    public class OracleCommandManager
    {
        private OracleCommand _insert, _delete, _select, _update;
        private String buscar;
        private OracleConnection con;
        /// <summary>
        /// Devuelve una instancia de OracleCommandManager con la conexion (OracleConection) dada.
        /// </summary>
        public OracleCommandManager(OracleConnection conexion)
        {
            this.con = conexion;
        }
        #region Comandos
        /// <summary>
        /// El metodo Insert solicita dos parametros: el objeto a ser insertado y un boolean especificando si el identificacor del objeto se genera automaticamente desde la base de datos. Por defecto este ultimo es verdadero.
        /// </summary>
        public async Task<bool> Insert<T>(T objeto, bool autoId = true) where T : class, new()
        {
            var miembros = typeof(T).GetProperties();
            String tabla = typeof(T).Name;
            FormatearComando();
            String val = "";
            SetFieldsForInsert<T>(out val, objeto);
            if (autoId)
            {
                val = "null," + val;
            }
            else
            {
                if (miembros[0].GetValue(objeto) is String)
                {
                    val = "'"+ miembros[0].GetValue(objeto) + "'," + val;
                }
                else
                {
                    val = miembros[0].GetValue(objeto).ToString() + "," + val;
                }
            }
            try
            {
                _insert = new OracleCommand();
                _insert.Connection = con;
                _insert.CommandType = CommandType.StoredProcedure;
                _insert.CommandText = "SP_INSERTAR";
                _insert.Parameters.Add("tabla",OracleDbType.Varchar2,tabla,ParameterDirection.Input);
                _insert.Parameters.Add("valores",OracleDbType.Varchar2,val,ParameterDirection.Input);
                if ((await _insert.ExecuteNonQueryAsync()) < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public async Task<T> Get<T>(object id) where T : class, new()
        {
            FormatearComando();
            String tabla = typeof(T).Name;
            String condicion ="";
            String val ="";
            SetFieldsForSelect<T>(out condicion, out val, id,typeof(T).GetProperties()[0],id is String);
            _select = new OracleCommand("SP_SELECCIONAR", con);
            try
            {
                _select.CommandType = CommandType.StoredProcedure;
                _select.Parameters.Add("tabla", OracleDbType.Varchar2, tabla, ParameterDirection.Input);
                _select.Parameters.Add("valores", OracleDbType.Varchar2, val, ParameterDirection.Input);
                _select.Parameters.Add("condicion", OracleDbType.Varchar2, condicion, ParameterDirection.Input);
                _select.Parameters.Add("retorno", OracleDbType.RefCursor, ParameterDirection.Output);
                OracleDataReader dReader = (OracleDataReader)await _select.ExecuteReaderAsync();
                Object[] obj = new Object[typeof(T).GetProperties().Length];
                while (dReader.Read())
                {
                    dReader.GetValues(obj);
                }
                dReader.Close();
                if (obj[0] == null)
                {
                    return null;
                }
                else
                {
                    T t = new T();
                    var m = typeof(T).GetProperties();
                    int l = 0;
                    foreach (var item in m)
                    {
                        if (!(obj[l] is DBNull))
                        {
                            try
                            {
                                item.SetValue(t, obj[l]);
                            }
                            catch (ArgumentException e)
                            {
                                try
                                {
                                    item.SetValue(t, Convert.ToChar(obj[l]));
                                }
                                catch (OverflowException ex)
                                {
                                    item.SetValue(t, Convert.ToInt32(obj[l]));
                                }
                            }
                        }
                        l++;
                    }
                    return t;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<List<T>> GetAll<T>() where T : class, new()
        {
            String tabla = typeof(T).Name;
            FormatearComando();
            String val = "";
            SetFieldsForAll(out val);
            buscar = buscar.Replace("VALORES", val);
            buscar = buscar.Replace("TABLA", tabla);
            buscar = buscar.Replace("WHERE CONDICION", "");
            Console.WriteLine(buscar);
            try
            {
                _select = new OracleCommand(buscar, con);
                _select.CommandType = CommandType.Text;
                OracleDataReader dReader = (OracleDataReader)await _select.ExecuteReaderAsync();
                OracleDataReader dr = (OracleDataReader)await _select.ExecuteReaderAsync();
                int cuentaObjetos = 0;
                while (dr.Read())
                {
                    cuentaObjetos++;
                }
                dr.Close();
                List<T> lista = new List<T>();
                Object[][] obj = new Object[cuentaObjetos][];
                int l = 0;
                while (dReader.Read())
                {
                    obj[l] = new Object[typeof(T).GetProperties().Length];
                    for(int j = 0;j< typeof(T).GetProperties().Length; j++)
                    {
                        obj[l][j] = dReader.GetValue(j);
                    }
                    l++;
                }
                dReader.Close();
                T t;
                var m = typeof(T).GetProperties();
                foreach (var ob in obj)
                {
                    l = 0;
                    t = new T();
                    foreach (var item in m)
                    {
                        if(!(ob[l] is DBNull))
                        {
                            try
                            {
                                item.SetValue(t, ob[l]);
                            }
                            catch (ArgumentException e)
                            {
                                try
                                {
                                    Char c = Char.Parse((String)ob[l]);
                                    item.SetValue(t, c);
                                }
                                catch (ArgumentException ex)
                                {
                                    bool b = ((String)ob[l]).Equals("1");
                                    item.SetValue(t, b);
                                }
                            }
                        }
                        l++;
                    }
                    lista.Add(t);
                }
                return lista;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<T>();
            }
        }
        public async Task<bool> Update<T>(T objeto) where T : class, new()
        {
            String tabla = typeof(T).Name;
            var miembros = typeof(T).GetProperties();
            bool b = miembros[0].GetValue(objeto) is String;
            FormatearComando();
            String val = "";
            String condicion = "";
            SetFieldsForUpdate<T>(out condicion,out val,objeto:objeto,idIsString:b);
            try
            {
                _update = new OracleCommand();
                _update.Connection = con;
                _update.CommandType = CommandType.StoredProcedure;
                _update.CommandText = "SP_ACTUALIZAR";
                _update.Parameters.Add("tabla", OracleDbType.Varchar2, tabla, ParameterDirection.Input);
                _update.Parameters.Add("valores", OracleDbType.Varchar2, val, ParameterDirection.Input);
                _update.Parameters.Add("condicion", OracleDbType.Varchar2, condicion, ParameterDirection.Input);
                if (await _update.ExecuteNonQueryAsync() < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return false;
            }

        }
        public async Task<bool> Delete<T>(T objeto) where T : class, new()
        {
            String tabla = typeof(T).Name;
            var miembros = typeof(T).GetProperties();
            FormatearComando();
            String condicion = "";
            if(miembros[0].GetValue(objeto) is String)
            {
                condicion = FormatId(miembros[0].Name, miembros[0].GetValue(objeto), true);
            }
            else
            {
                condicion = FormatId(miembros[0].Name, miembros[0].GetValue(objeto));
            }
            try
            {
                _delete = new OracleCommand();
                _delete.Connection = con;
                _delete.CommandType = CommandType.StoredProcedure;
                _delete.CommandText = "SP_ELIMINAR";
                _delete.Parameters.Add("tabla", OracleDbType.Varchar2, tabla, ParameterDirection.Input);
                _delete.Parameters.Add("condicion", OracleDbType.Varchar2, condicion, ParameterDirection.Input);
                if (await _delete.ExecuteNonQueryAsync() < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return false;
            }
        }
        public async Task<List<T>> Find<T>(String field, object value) where T : class, new()
        {
            FormatearComando();
            String tabla = typeof(T).Name;
            String condicion = FormatId(field, value, value is String);
            buscar = buscar.Replace("VALORES", "*");
            buscar = buscar.Replace("TABLA", tabla);
            buscar = buscar.Replace("CONDICION", condicion);
            Console.WriteLine(buscar);
            try
            {
                _select = new OracleCommand(buscar, con);
                _select.CommandType = CommandType.Text;
                OracleDataReader dReader = (OracleDataReader)await _select.ExecuteReaderAsync();
                OracleDataReader dr = (OracleDataReader)await _select.ExecuteReaderAsync();
                int cuentaObjetos = 0;
                while (dr.Read())
                {
                    cuentaObjetos++;
                }
                dr.Close();
                List<T> lista = new List<T>();
                Object[][] obj = new Object[cuentaObjetos][];
                int l = 0;
                while (dReader.Read())
                {
                    obj[l] = new Object[typeof(T).GetProperties().Length];
                    for (int j = 0; j < typeof(T).GetProperties().Length; j++)
                    {
                        obj[l][j] = dReader.GetValue(j);
                    }
                    l++;
                }
                dReader.Close();
                T t;
                var m = typeof(T).GetProperties();
                foreach (var ob in obj)
                {
                    l = 0;
                    t = new T();
                    foreach (var item in m)
                    {
                        if (!(ob[l] is DBNull))
                        {
                            try
                            {
                                item.SetValue(t, ob[l]);
                            }
                            catch (ArgumentException e)
                            {
                                try
                                {
                                    Char c = Char.Parse((String)ob[l]);
                                    item.SetValue(t, c);
                                }
                                catch (ArgumentException ex)
                                {
                                    bool b = ((String)ob[l]).Equals("1");
                                    item.SetValue(t, b);
                                }
                            }
                        }
                        l++;
                    }
                    lista.Add(t);
                }
                return lista;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<T>();
            }
        }
        #endregion


        private void FormatearComando()
        {
            buscar = "SELECT VALORES FROM TABLA WHERE CONDICION";

        }
        private void SetFieldsForSelect<T>(out String id, out String valores, object idValue,PropertyInfo pi, bool idIsString = false) where T : class
        {//SELECT
            SetFieldsForAll(out valores);
            id = FormatId(pi.Name, idValue, idIsString);
        }
        private void SetFieldsForAll(out String valores)
        {//SELECT ALL
            valores = "*";
        }
        private void SetFieldsForUpdate<T>(out String id,out String valores, T objeto, bool idIsString = false) where T : class
        {//UPDATE
            var miembros = typeof(T).GetProperties();
            SetFieldsForInsert<T>(out valores,objeto,true);
            var idValue = miembros[0].GetValue(objeto);
            id = FormatId(miembros[0].Name, idValue, idIsString);
        }
        private void SetFieldsForInsert<T>(out String valores, T objeto, bool update = false) where T : class
        {//INSERT
            var miembros = typeof(T).GetProperties();
            valores = "";
            int i = 0;
            String[] vals = new String[miembros.Length];
            foreach(var f in miembros)
            {
                if (f.GetValue(objeto) == null)
                {
                    vals[i] = null;
                }
                else
                {
                    if (f.GetValue(objeto) is String)
                    {
                        vals[i] = "'" + f.GetValue(objeto).ToString() + "'";
                    }
                    if (f.GetValue(objeto) is int || f.GetValue(objeto) is short 
                        || f.GetValue(objeto) is Int32 || f.GetValue(objeto) is Int64
                        || f.GetValue(objeto) is float || f.GetValue(objeto) is double)
                    {
                        vals[i] = f.GetValue(objeto).ToString();
                    }
                    if (f.GetValue(objeto) is char)
                    {
                        vals[i] = "'"+f.GetValue(objeto).ToString()+"'";
                    }
                    if (f.GetValue(objeto) is bool)
                    {
                        vals[i] = ((bool)f.GetValue(objeto)) ? "'1'" : "'0'";
                    }
                    if (f.GetValue(objeto) is DateTime)
                    {
                        vals[i] = "TO_DATE('" + Tools.DateToString((DateTime)f.GetValue(objeto),DateFormat.YearMonthDay,'-') + "','YYYY-MM-DD')";
                    }
                }
                i++;
            }
            i = 0;
            //ANULAR LA ID PORQUE LA GENERA LA BDD O ES CONDICION
            foreach (var inf in miembros)
            {
                if (i > 0)
                {
                    if (update)
                    {
                        if (vals[i] != null)
                        {
                            valores += inf.Name + "=" + vals[i] + ",";
                        }
                    }
                    else
                    {
                        if (vals[i] != null)
                        {
                            valores += vals[i] + ",";
                        }
                        else
                        {
                            valores += "null,";
                        }
                    }
                }
                i++;
            }
            valores = valores.Substring(0, valores.Length - 1);
        }
        private String FormatId(String field, dynamic idValue,bool idIsString = false)
        {
            if (idIsString)
            {
                return field + "='" + idValue + "'";
            }
            else
            {
                return field + "=" + idValue.ToString();
            }
        }
    }
}