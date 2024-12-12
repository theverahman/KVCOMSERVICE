using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace KVCOMSERVICE
{
    public partial class Service1 : ServiceBase
    {
        private readonly ServiceExecutableSource _executableSource;
        private int _checkInterval;
        private Process _process;
        private Timer _timer;

        public Service1(ServiceExecutableSource executableSource, string[] args)
        {
            InitializeComponent();
            _executableSource = executableSource;
            _checkInterval = GetCheckIntervalFromArguments(args);
        }

        public void StartService(string[] args)
        {
            OnStart(args);
        }

        public void StopService()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            StartProcess();
            _timer = new Timer(CheckProcess, null, _checkInterval, _checkInterval);
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
                        FileName = _executableSource.GetExecutablePath(),
                        WorkingDirectory = _executableSource.GetExecutableFolder(),
                        UseShellExecute = false,
                        CreateNoWindow = false
                    }
                };

                _process.Start();
            }
        }

        private void StopProcess()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.Dispose();
                _process = null;
            }
        }

        private void CheckProcess(object state)
        {
            if (_process == null || _process.HasExited)
            {
                StartProcess();
            }
        }

        private int GetCheckIntervalFromArguments(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("/checkinterval:"))
                {
                    string value = arg.Substring(14);
                    if (int.TryParse(value, out int interval))
                    {
                        return interval;
                    }
                }
            }
            return 5000; // default value
        }
    }
}