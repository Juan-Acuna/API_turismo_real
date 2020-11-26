using API_TurismoReal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_TurismoReal.Conexiones
{
    public interface IProxyInforme
    {
        ItemInforme Add(ItemInforme item);
    }
    public abstract class ItemInforme
    {
        public abstract long CalcularTotales();
    }

    public class Informe
    {
        public String mes { get; private set; }
        int numMes;
        public int ano { get; private set; }
        public ProxyIngresos Ingresos { get; private set; }
        public ProxyEgresos Egresos { get; private set; }
        public ProxyUtilidades Utilidades { get; private set; }

        public Informe()
        {
            ano = 0;
            Ingresos = new ProxyIngresos();
            Egresos = new ProxyEgresos();
            Utilidades = new ProxyUtilidades();
        }
        public Informe(int mes, int ano)
        {
            switch (mes)
            {
                case 1:
                    this.mes = "enero";
                    break;
                case 2:
                    this.mes = "febrero";
                    break;
                case 3:
                    this.mes = "marzo";
                    break;
                case 4:
                    this.mes = "abril";
                    break;
                case 5:
                    this.mes = "mayo";
                    break;
                case 6:
                    this.mes = "junio";
                    break;
                case 7:
                    this.mes = "julio";
                    break;
                case 8:
                    this.mes = "agosto";
                    break;
                case 9:
                    this.mes = "septiembre";
                    break;
                case 10:
                    this.mes = "octubre";
                    break;
                case 11:
                    this.mes = "noviembre";
                    break;
                case 12:
                    this.mes = "diciembre";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mes");
            }
            if(ano < 1000 || ano > 9999)
            {
                throw new ArgumentOutOfRangeException("año ('ano')");
            }
            this.ano=ano;
            this.numMes = mes;
            Ingresos = new ProxyIngresos();
            Egresos = new ProxyEgresos();
            Utilidades = new ProxyUtilidades();
        }
        public void CargarDeptos(List<Departamento> deptos)
        {
            foreach(var d in deptos)
            {
                Ingresos.Add(new IngresoReserva("["+d.Id_depto.ToString() + "]" + d.Nombre, d.Arriendo));
                Egresos.Add(new EgresoDepto("[" + d.Id_depto.ToString() + "]" + d.Nombre, d.Dividendo, d.Contribuciones));
            }
        }
        public void CargarServicios(List<Servicio> servicios)
        {
            foreach(var s in servicios)
            {
                Ingresos.Add(new IngresoServicio("[" + s.Id_servicio.ToString() + "]" + s.Nombre,s.Valor));
            }
        }
        public void Procesar(List<Reserva> reservas, List<ReservaServicio> rservicios, List<Mantencion> mantenciones, List<TipoMantencion> tipos)
        {
            List<Reserva> rs = new List<Reserva>();
            List<ReservaServicio> sers = new List<ReservaServicio>();
            //filtrado de reservas
            foreach (var r in reservas)
            {
                if (r.Inicio_estadia.Month == numMes && r.Inicio_estadia.Year == ano)
                {
                    rs.Add(r);
                }
            }
            //filtrado de servicios
            foreach (var r in rs)
            {
                foreach (var s in rservicios)
                {
                    if (r.Id_reserva == s.Id_reserva)
                    {
                        sers.Add(s);
                    }
                }
            }
            //ASIGNACIONES
            //reservas
            for (int i = 0; i < Ingresos.IngresosReserva.Count; i++)
            {
                foreach(var r in rs)
                {
                    if (r.Id_depto == Int32.Parse(Ingresos.IngresosReserva[i].Depto.Split("]")[0].Replace("[", "")))
                    { 
                        Ingresos.IngresosReserva[i].Reservas++;
                        Ingresos.IngresosReserva[i].DiasTotales += (r.Fin_estadia - r.Inicio_estadia).Days;
                    }
                }
                Ingresos.IngresosReserva[i].CalcularTotales();
            }
            //servicios
            for(int i = 0; i < Ingresos.IngresosServicio.Count; i++)
            {
                foreach(var s in sers)
                {
                    if(s.Id_servicio == Int32.Parse(Ingresos.IngresosServicio[i].Servicio.Split("]")[0].Replace("[", "")))
                    {
                        Ingresos.IngresosServicio[i].Contrataciones++;
                    }
                }
                Ingresos.IngresosServicio[i].CalcularTotales();
            }
            //mantenciones
            for(int i = 0; i < Egresos.EgresosDepto.Count; i++)
            {
                foreach(var m in mantenciones)
                {
                    if(m.Id_depto == Int32.Parse(Egresos.EgresosDepto[i].Depto.Split("]")[0].Replace("[", "")))
                    {
                        Egresos.EgresosDepto[i].Mantenciones += Tools.BuscarEnLista(tipos,"Id_tipo",m.Id_tipo).Valor;
                    }
                }
                Egresos.EgresosDepto[i].CalcularTotales();
            }
            //utilidades
            for(int i = 0; i < Egresos.EgresosDepto.Count; i++)
            {
                Utilidades.Utilidades.Add(new Utilidad(Ingresos.IngresosReserva[i], Egresos.EgresosDepto[i]));
            }
        }
        public String ImprimirInforme()
        {
            String s = "";

            return s;
        }
    }
    public class ProxyIngresos : IProxyInforme
    {
        public List<IngresoReserva> IngresosReserva { get; private set; }
        public List<IngresoServicio> IngresosServicio { get; private set; }

        public ProxyIngresos()
        {
            IngresosReserva = new List<IngresoReserva>();
            IngresosServicio = new List<IngresoServicio>();
        }
        public ItemInforme Add(ItemInforme item)
        {
            if(item is IngresoReserva)
            {
                IngresosReserva.Add(item as IngresoReserva);
            }
            else if(item is IngresoServicio)
            {
                IngresosServicio.Add(item as IngresoServicio);
            }
            else
            {
                throw new ArgumentException();
            }
            return item;
        }
    }
    public class ProxyEgresos : IProxyInforme
    {
        public List<EgresoDepto> EgresosDepto { get; private set; }

        public ProxyEgresos()
        {
            EgresosDepto = new List<EgresoDepto>();
        }
        public ItemInforme Add(ItemInforme item)
        {
            if(item is EgresoDepto)
            {
                EgresosDepto.Add(item as EgresoDepto);
            }
            else
            {
                throw new ArgumentException();
            }
            return item;
        }
    }
    public class ProxyUtilidades : IProxyInforme
    {
        public List<Utilidad> Utilidades { get; private set; }

        public ProxyUtilidades()
        {
            Utilidades = new List<Utilidad>();
        }
        public ItemInforme Add(ItemInforme item)
        {
            if(item is Utilidad)
            {
                Utilidades.Add(item as Utilidad);
            }
            else
            {
                throw new ArgumentException();
            }
            return item;
        }
    }
    public class IngresoReserva : ItemInforme
    {
        public String Depto { get; private set; }
        public int CostoDia { get; private set; }
        public int Reservas { get; set; }
        public int DiasTotales { get; set; }
        public long Ganancias { get; private set; }
        public IngresoReserva(String depto, int costoDia)
        {
            Reservas = 0;
            DiasTotales = 0;
            Depto = depto;
            CostoDia = costoDia;
        }

        public override long CalcularTotales()
        {
            Ganancias = CostoDia * DiasTotales;
            return Ganancias;
        }
    }
    public class IngresoServicio : ItemInforme
    {
        public String Servicio { get; private set; }
        public int CostoContratacion { get; private set; }
        public int Contrataciones { get; set; }
        public long Ganancias { get; private set; }
        public IngresoServicio(String servicio, int costo)
        {
            Contrataciones = 0;
            Servicio = servicio;
            CostoContratacion = costo;
        }

        public override long CalcularTotales()
        {
            Ganancias = CostoContratacion * Contrataciones;
            return Ganancias;
        }
    }
    public class EgresoDepto : ItemInforme
    {
        public String Depto { get; private set; }
        public int Dividendo { get; private set; }
        public int Contribuciones { get; private set; }
        public long Mantenciones { get; set; }
        public long GastoTotal { get; private set; }
        public EgresoDepto(String depto, int dividendo, int contribuciones)
        {
            Mantenciones = 0;
            Depto = depto;
            Dividendo = dividendo;
            Contribuciones = contribuciones;
        }

        public override long CalcularTotales()
        {
            GastoTotal = Dividendo + Contribuciones + Mantenciones;
            return GastoTotal;
        }
    }
    public class Utilidad : ItemInforme
    {
        public String Depto { get; private set; }
        public long CostoMantencion { get; private set; }
        public long GananciasReservas { get; private set; }
        public long TotalUtilidades { get; private set; }
        public Utilidad(IngresoReserva ingreso, EgresoDepto egreso)
        {
            Depto = ingreso.Depto;
            CostoMantencion = egreso.GastoTotal;
            GananciasReservas = ingreso.Ganancias;
            CalcularTotales();
        }

        public override long CalcularTotales()
        {
            TotalUtilidades = GananciasReservas - CostoMantencion;
            return TotalUtilidades;
        }
    }
}
