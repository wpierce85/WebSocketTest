using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebSocket.PMGo
{
    public class Program
    {
        #region Public Methods

        public static void Main(string[] args)
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        #endregion
    }
}