using FS.Core.Services;
using log4net;
using System;

namespace FS.Infrastructure.Services
{
    public class LogService : ILogService
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static LogService()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        #region ILoggingService Members

        /// <summary>
        /// Use this for Debug messages
        /// </summary>
        public void Debug(string format)
        {
            _log.Debug(format);
        }
        /// <summary>
        /// Use this for Debug messages. Use object array do log any other variables
        /// </summary>
        public void Debug(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        /// <summary>
        /// Use this for Error messages.
        /// </summary>
        public void Error(string format)
        {
            _log.Error(format);
        }
        /// <summary>
        /// Use this for Error messages. Use object array do log any other variables
        /// </summary>
        public void Error(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }
        /// <summary>
        /// Use this for Error messages. Use Exception to pass on any exception (inner exceptions maybe)
        /// </summary>        
        public void Error(string format, Exception exception)
        {
            _log.Error(format, exception);
        }

        /// <summary>
        /// Use this for Fatal messages. Fatal message would be anything that critical and needs immediate support
        /// Like missing parameter in config files or some other events that prevent application from running properly
        /// </summary>                
        public void Fatal(string format)
        {
            _log.Fatal(format);
        }
        /// <summary>
        /// Use this for Fatal messages. Fatal message would be anything that critical and needs immediate support
        /// Like missing parameter in config files or some other events that prevent application from running properly
        /// </summary>                        
        public void Fatal(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }
        /// <summary>
        /// Use this for Fatal messages. Fatal message would be anything that critical and needs immediate support
        /// Like missing parameter in config files or some other events that prevent application from running properly
        /// </summary>                        
        public void Fatal(string format, Exception exception)
        {
            _log.Fatal(format, exception);
        }

        /// <summary>
        /// This is could be used for tracing or audit function.
        /// </summary>                        
        public void Info(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        /// <summary>
        /// This is could be used for tracing or audit function.
        /// </summary>                        
        public void Info(string format)
        {
            _log.Info(format);
        }

        /// <summary>
        /// Custom method
        /// </summary>
        /// <param name="methodName">name of method error was caught</param>
        /// <param name="parameters">parameters to be printed with comma delimiter</param>
        /// <param name="ex">System exception thrown</param>
        public void CatchException(string methodName, object[] parameters, Exception ex)
        {
            Error(string.Format("{0} {1}({2});  {3}", this.ToString(), methodName, string.Join(",", parameters), ex.Message), ex);
        }

        #endregion
    }
}
