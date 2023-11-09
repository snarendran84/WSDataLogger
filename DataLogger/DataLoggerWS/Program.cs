using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerWS
{
    internal static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            logger.Info("Kickstarting main program now");
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new DataLoggerService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }
    }
}
