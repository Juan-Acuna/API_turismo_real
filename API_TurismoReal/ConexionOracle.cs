using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal
{
    public class ConexionOracle
    {
        private const String SOURCE = "localhost:1521";//OFFLINE
        //private const String SOURCE = "85.208.***.***:1521";//ONLINE
        private static String USER = "turismo_real";
        private static String PASSWD = "432";
        private static String CON_STR = "DATA SOURCE=" + SOURCE + ";USER ID=" + USER + ";PASSWORD=" + PASSWD + ";";
        public static OracleConnection _con = new OracleConnection(CON_STR);
        private static ConexionOracle _instancia = new ConexionOracle();
        static bool activa;
        public static bool Activa { get { return activa; } }

        private ConexionOracle()
        {
            activa = false;
            Open();
        }

        public static OracleConnection Conexion
        {
            get
            {
                if (_con != null)
                {
                    return _con;
                }
                else
                {
                    return _con = new OracleConnection(CON_STR);
                }
            }
        }
        public static void Close()
        {
            if (activa)
            {
                _con.Close();
                activa = false;
            }
        }
        public static void Open()
        {
            if (!activa)
            {
                try
                {
                    _con.Open();
                    activa = true;
                }
                catch (Exception e)
                {
                    activa = false;
                }
            }
        }
    }
}
