using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API_TurismoReal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                using (var stream = System.IO.File.Create(Secret.RUTA_RAIZ + "error.txt"))
                {
                    stream.Write(Encoding.UTF8.GetBytes("Error:\n"+e.Message+";\nInnerException:\n"+e.InnerException+";\nSource:\n"+e.Source+";\nTarget:\n"+e.TargetSite));
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000;https://turismoreal.xyz:8080;http://turismoreal.xyz:8080")
                .UseStartup<Startup>();
    }
}
