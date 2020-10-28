using API_TurismoReal.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public class ConexionOracle
    {
        //private const String SOURCE = "localhost:1521";//OFFLINE
        private static String SOURCE = Secret.SERVIDOR;//ONLINE
        private static String USER = Secret.USUARIO;
        private static String PASSWD = Secret.CLAVE;
        static String CON_STR = "DATA SOURCE=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = " + SOURCE + ")(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = TREALDB)));USER ID=" + USER + ";PASSWORD="+PASSWD;
        public static OracleConnection _con = new OracleConnection(CON_STR);
        public static OracleConnection _con2 = new OracleConnection(Secret.CONEXION_ANTIGUA);
        public static OracleConnection con;
        static bool con1 = false;
        private static ConexionOracle _instancia = new ConexionOracle();
        static bool activa = false;
        static bool activa2 = false;
        static String orig = "Oracle XE";
        public static String Origen { get { return orig; } }
        public static bool Activa
        {
            get
            {
                if (con1)
                {
                    return activa;
                }
                else
                {
                    return activa2;
                }
            }
        }

        private ConexionOracle()
        {
            try
            {
                _con2.Open();
                activa2 = true;
                _con.Open();
                activa = true;
            }
            catch (Exception e) { }
            con = _con2;
            //activa = false;
            //Open();
        }

        public static String Switch()
        {
            if (con1)
            {
                con1 = false;
                con = _con2;
                orig = "Oracle XE";
                return "Trabajando con Oracle XE.";
            }
            else
            {
                con1 = true;
                con = _con;
                orig = "Oracle Standard One";
                return "Trabajando con Oracle Standard One.";
            }
        }

        public static OracleConnection Conexion
        {
            get
            {
                if (con != null)
                {
                    return con;
                }
                else
                {
                    return con = _con;
                }
            }
        }
        public static void Close()
        {
            if (activa)
            {
                con.Close();
                activa = false;
            }
        }
        public static void Open()
        {
            if (!Activa)
            {
                try
                {
                    con.Open();
                    if (con1)
                    {
                        activa = true;
                    }
                    else
                    {
                        activa2 = true;
                    }
                }
                catch (InvalidOperationException i)
                {
                    if (con1)
                    {
                        activa = true;
                    }
                    else
                    {
                        activa2 = true;
                    }
                }
                catch (Exception e)
                {
                    if (con1)
                    {
                        activa = false;
                    }
                    else
                    {
                        activa2 = false;
                    }
                }
            }
        }
        public static MensajeError NoConResponse
        {
            get
            {
                return MensajeError.Nuevo("No se pudo establecer comunicacion con la base de datos");
            }
        }
    }
    /*public class ConexionOracle
    {
        //private const String SOURCE = "localhost:1521";//OFFLINE
        private static String SOURCE = Temp.SERVIDOR;//ONLINE
        private static String USER = Temp.USUARIO;
        private static String PASSWD = Temp.CLAVE;
        private static String CON_STR = "DATA SOURCE=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(HOST = " + SOURCE + ")(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = TREALDB)));USER ID=" + USER + ";PASSWORD="+PASSWD;
        public static OracleConnection _con = new OracleConnection(CON_STR);
        static bool con1 = false;
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
                con.Close();
                activa = false;
            }
        }
        public static void Open()
        {
            if (!activa)
            {
                try
                {
                    con.Open();
                    activa = true;
                }
                catch (InvalidOperationException i)
                {
                    activa = true;
                }
                catch (Exception e)
                {
                    activa = false;
                }
            }
        }
        public static MensajeError NoConResponse
        {
            get
            {
                return MensajeError.Nuevo("No se pudo establecer comunicacion con la base de datos");
            }
        }
    }*/
}
