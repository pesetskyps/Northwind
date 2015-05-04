using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Northwind.Infrastructure
{
    public class Logger
    {
        private static readonly Lazy<Logger> _logger =
    new Lazy<Logger>(() => new Logger());
        private ILog _log;
        public static Logger Instance { get { return _logger.Value; } }

        private Logger()
        {
        }

        private void Initialize()
        {
            _log = LogManager.GetLogger(typeof(Logger));
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Error(Exception ex)
        {
            if (_log == null)
            {
                Initialize();
            }

            _log.Error(ex);
        }

        public void Debug(string message)
        {
            if (_log == null)
            {
                Initialize();
            }
            _log.Debug(string.Format("Debug:{0}", message));
        }
    }
}
