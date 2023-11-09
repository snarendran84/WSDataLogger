using DataLoggerWS.core;
using NLog;
using NLog.Fluent;
using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace DataLoggerWS
{
    public partial class DataLoggerService : ServiceBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        protected FileSystemWatcher fileWatcher;
        string monitoringFolder = ConfigurationManager.AppSettings["MonitoringFolderPath"];
        int interval = int.Parse(ConfigurationManager.AppSettings["interval"]);
        string ErrorLogFile = ConfigurationManager.AppSettings["CriticalErrorPath"];

        private Timer timer;

        public DataLoggerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("DataLogger Service is getting started");
            try
            {
                timer = new Timer(new TimerCallback(StartMonitoring), null, 0, interval);
                base.OnStart(args);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while starting the DataLogger service");
            }
        }
        protected override void OnPause()
        {
            try
            {
                logger.Info("DataLogger Service is paused");
                fileWatcher.EnableRaisingEvents = false;
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                base.OnPause();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error when trying to pause the DataLogger service");
            }
        }

        protected override void OnContinue()
        {
            logger.Info("Resuming execution of DataLogger Service ");
            try
            {
                fileWatcher.EnableRaisingEvents = true;
                timer.Change(0, interval);
                base.OnContinue();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error when trying to resume DataLogger service");
            }
        }

        protected override void OnStop()
        {
            logger.Info("DataLogger Service is shut down");
            try
            {
                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
                timer.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error when stopping DataLogger service");
            }
        }

        private void StartMonitoring(object state)
        {
            try
            {
                fileWatcher = new DirectoryMonitor(monitoringFolder);
                fileWatcher.EnableRaisingEvents = true;
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error when monitoring the folder for changes");
            }
        }
    }
}
