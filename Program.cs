using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace KVCOMSERVICE
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                // If running in debug mode, run the service directly
                var service = new Service1(args.Length > 0 ? args[0] : string.Empty);
                service.StartService(args);
                Console.WriteLine("Press any key to stop the service...");
                Console.ReadKey();
                service.StopService();
            }
            else
            {
                // If running as a service, run the service normally
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service1(args.Length > 0 ? args[0] : string.Empty)
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
