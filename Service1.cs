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
                        CreateNoWindow = false,
                        RedirectStandardError = true
                    }
                };

                _process.ErrorDataReceived += Process_ErrorDataReceived;
                _process.Exited += Process_Exited;
                _process.Start();
                _process.BeginErrorReadLine();
            }
        }

        private void StopProcess()
        {
            if (_process != null && !_process.HasExited)
            {
                try
                {
                    _process.Kill();
                }
                catch (Exception ex)
                {
                    //EventLog.WriteEntry("Service", "Error stopping process: " + ex.Message, EventLogEntryType.Error);
                }
                finally
                {
                    _process.Dispose();
                    _process = null;
                }
            }
        }

        private void CheckProcess(object state)
        {
            if (_process == null || _process.HasExited)
            {
                StartProcess();
            }
            else
            {
                try
                {
                    if (_process.Responding == false)
                    {
                        StopProcess();
                        StartProcess();
                    }
                }
                catch (Exception ex)
                {
                    //EventLog.WriteEntry("Service", "Error checking process: " + ex.Message, EventLogEntryType.Error);
                    StopProcess();
                    StartProcess();
                }
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                string errorMessage = e.Data;
                if (errorMessage.Contains("Exception") || errorMessage.Contains("Error"))
                {
                    //EventLog.WriteEntry("Service", "Unhandled exception in process: " + errorMessage, EventLogEntryType.Error);
                    ShutdownAndRestartProcess();
                }
                else
                {
                    //EventLog.WriteEntry("Service", "Error in process: " + errorMessage, EventLogEntryType.Error);
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            int exitCode = _process.ExitCode;
            if (exitCode != 0)
            {
                //EventLog.WriteEntry("Service", "Process exited with code " + exitCode, EventLogEntryType.Error);
                ShutdownAndRestartProcess();
            }
            else
            {
                //EventLog.WriteEntry("Service", "Process exited normally", EventLogEntryType.Information);
            }
        }

        private void ShutdownAndRestartProcess()
        {
            StopProcess();
            StartProcess();
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
            return 10000; // default value
        }
    }
}