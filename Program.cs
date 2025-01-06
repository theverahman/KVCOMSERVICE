using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using System.ServiceProcess;

namespace KVCOMSERVICE
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                RunAsService(args);
            }
            else
            {
                string[] arguments = Environment.GetCommandLineArgs();
                RunAsService(arguments);
            }
            

            //RunInDebugMode(args);
            /*
            if (Environment.UserInteractive)
            {
                RunInDebugMode(args);
            }
            else
            {
                RunAsService(args);
            }
            */
        }

        private static void RunInDebugMode()
        {
            var args = GetExecutablePathForDebugMode();
            var service = new Service1(args);
            service.StartService(args);
            // Wait for the service to stop
        }

        private static void RunAsService(string[] args)
        {
            var service = new Service1(args);
            ServiceBase[] ServicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(ServicesToRun);
        }

        private static string[] GetExecutablePathForDebugMode()
        {
            return new string[]  { @"D:\PROJECT\VISUAL_STUDIO_PROJECTS\KVCOMSERVER\bin\Debug\net8.0-windows\KVCOMSERVER.exe" };
        }
    }
}