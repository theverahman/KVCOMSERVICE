using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Text;

namespace KVCOMSERVICE
{
    public partial class Service1 : ServiceBase
    {
        private Process _process;
        private Timer _timer;
        private string _exePath;

        public Service1(string exePath)
        {
            InitializeComponent();
            _exePath = exePath;
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length > 0)
            {
                _exePath = args[0]; // Get the executable path from the arguments
            }
            StartProcess();
            _timer = new Timer(CheckProcess, null, 1000, 1000); // Check every second
        }

        protected override void OnStop()
        {
            StopProcess();
            _timer?.Dispose();
        }

        private void StartProcess()
        {
            if (_process == null || _process.HasExited)
            {
                _process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _exePath,
                        UseShellExecute = false,
                        CreateNoWindow = true // Set to true if you want to run it in the background
                    }
                };

                _process.Start();
                EventLog.WriteEntry("ExeMonitorService", $"Started process: {_process.Id}", EventLogEntryType.Information);
            }
        }

        private void StopProcess()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.Dispose();
                _process = null;
                EventLog.WriteEntry("ExeMonitorService", "Stopped process.", EventLogEntryType.Information);
            }
        }

        private void CheckProcess(object state)
        {
            if (_process == null || _process.HasExited)
            {
                EventLog.WriteEntry("ExeMonitorService", "Process has exited. Restarting...", EventLogEntryType.Warning);
                StartProcess();
            }
        }
    }
}
