using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace KVCOMSERVICE
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            RunAsService(args);
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

        private static void RunInDebugMode(string[] args)
        {
            var executableSource = new ServiceExecutableSource(GetExecutablePathForDebugMode());
            var service = new Service1(executableSource, args);
            service.StartService(args);
            // Wait for the service to stop
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void RunAsService(string[] args)
        {
            var executableSource = new ServiceExecutableSource(GetExecutablePathFromArguments(args));
            var service = new Service1(executableSource, args);
            ServiceBase[] ServicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(ServicesToRun);
        }

        private static string GetExecutablePathForDebugMode()
        {
            return @"D:\PROJECT\VISUAL_STUDIO_PROJECTS\KVCOMSERVER\bin\Debug\net8.0-windows\KVCOMSERVER.exe";
        }

        private static string GetExecutablePathFromArguments(string[] args)
        {
            if (args.Length > 0)
            {
                return args[0];
            }
            else
            {
                throw new ArgumentException("Executable path not provided as an argument.");
            }
        }
    }
}