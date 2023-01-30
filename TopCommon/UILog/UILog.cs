using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using TopCom.Define;
using System.Windows;

namespace TopCom.LOG
{
    public enum LogLevel
    {
        Debug = 0,
        Error = 1,
        Fatal = 2,
        Info = 3,
        Warning = 4
    }
    /// <summary>
    /// Write out messages using the logging provider.
    /// </summary>
    public static class UILog
    {
        #region Members
        private static readonly ILog _logger = LogManager.GetLogger("UILog");
        private static Dictionary<LogLevel, Action<string>> _actions;
        #endregion

        /// <summary>
        /// Static instance of the log manager.
        /// </summary>
        static UILog()
        {
            LogFactory.Configure();
            //XmlConfigurator.Configure();
            _actions = new Dictionary<LogLevel, Action<string>>();
            _actions.Add(LogLevel.Debug, Debug);
            _actions.Add(LogLevel.Error, Error);
            _actions.Add(LogLevel.Fatal, Fatal);
            _actions.Add(LogLevel.Info, Info);
            _actions.Add(LogLevel.Warning, Warning);
        }

        /// <summary>
        /// Get the <see cref="NotifyAppender"/> log.
        /// </summary>
        /// <returns>The instance of the <see cref="NotifyAppender"/> log, if configured. 
        /// Null otherwise.</returns>
        public static NotifyAppender Appender
        {
            get
            {
                try
                {
                    foreach (ILog log in LogManager.GetCurrentLoggers())
                    {
                        foreach (IAppender appender in log.Logger.Repository.GetAppenders())
                        {
                            if (appender is NotifyAppender)
                            {
                                return appender as NotifyAppender;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Write the message to the appropriate log based on the relevant log level.
        /// </summary>
        /// <param name="level">The log level to be used.</param>
        /// <param name="message">The message to be written.</param>
        /// <exception cref="ArgumentNullException">Thrown if the message is empty.</exception>
        public static void Write(LogLevel level, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (level > LogLevel.Warning || level < LogLevel.Debug)
                    throw new ArgumentOutOfRangeException("level");

                // Now call the appropriate log level message.
                _actions[level](message);
            }
        }

        #region Action methods
        public static void Debug(string message)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message);
        }

        public static void Error(string message)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message);
        }

        public static void Fatal(string message)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(message);
        }

        public static void Info(string message)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message);
        }

        public static void Warning(string message)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message);
        }
        #endregion
    }
}
