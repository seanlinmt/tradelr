///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Config;

namespace com.mosso.cloudfiles.utils
{
    /// <summary>
    /// the logging class
    /// </summary>
    public static class Log
    {
        private static readonly Dictionary<Type, ILog> _loggers = new Dictionary<Type, ILog>();
        private static bool logInitialized;
        private static readonly object _lock = new object();

        /// <summary>
        /// serialize the exception
        /// </summary>
        /// <param name="e">exception to serialize</param>
        /// <returns></returns>
        public static string SerializeException(Exception e)
        {
            return SerializeException(e, string.Empty);
        }

        private static string SerializeException(Exception e, string exceptionMessage)
        {
            if (e == null) return string.Empty;

            exceptionMessage = string.Format(
                "{0}{1}{2}\n{3}",
                exceptionMessage,
                (exceptionMessage == string.Empty) ? string.Empty : "\n\n",
                e.Message,
                e.StackTrace);

            if (e.InnerException != null)
                exceptionMessage = SerializeException(e.InnerException, exceptionMessage);

            return exceptionMessage;
        }

        private static ILog getLogger(Type source)
        {
            lock (_lock)
            {
                if (_loggers.ContainsKey(source))
                {
                    return _loggers[source];
                }

                ILog logger = LogManager.GetLogger(source);
                _loggers.Add(source, logger);
                return logger;
            }
        }

        /* Log a message object */

        /// <summary>
        /// log a message as a debug message
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        public static void Debug(object source, object message)
        {
            Debug(source.GetType(), message);
        }

        /// <summary>
        /// log a message as a debug message
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        public static void Debug(Type source, object message)
        {
            getLogger(source).Debug(message);
        }

        /// <summary>
        /// log a message as an info message
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        public static void Info(object source, object message)
        {
            Info(source.GetType(), message);
        }

        /// <summary>
        /// log a message as an info message
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        public static void Info(Type source, object message)
        {
            getLogger(source).Info(message);
        }

        /// <summary>
        /// log a message as a warn message
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        public static void Warn(object source, object message)
        {
            Warn(source.GetType(), message);
        }

        /// <summary>
        /// log a message as a warn message
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        public static void Warn(Type source, object message)
        {
            getLogger(source).Warn(message);
        }

        /// <summary>
        /// log a message as an error message
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        public static void Error(object source, object message)
        {
            Error(source.GetType(), message);
        }

        /// <summary>
        /// log a message as an error message
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        public static void Error(Type source, object message)
        {
            getLogger(source).Error(message);
        }

        /// <summary>
        /// log a message as a fatal message
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        public static void Fatal(object source, object message)
        {
            Fatal(source.GetType(), message);
        }

        /// <summary>
        /// log a message as a fatal message
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        public static void Fatal(Type source, object message)
        {
            getLogger(source).Fatal(message);
        }

        /* Log a message object and exception */

        /// <summary>
        /// log a message as a debug message with an exception
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Debug(object source, object message, Exception exception)
        {
            Debug(source.GetType(), message, exception);
        }

        /// <summary>
        /// log a message as a debug message with an exception
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Debug(Type source, object message, Exception exception)
        {
            getLogger(source).Debug(message, exception);
        }

        /// <summary>
        /// log a message as an info message with an exception
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Info(object source, object message, Exception exception)
        {
            Info(source.GetType(), message, exception);
        }

        /// <summary>
        /// log a message as an info message with an exception
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Info(Type source, object message, Exception exception)
        {
            getLogger(source).Info(message, exception);
        }

        /// <summary>
        /// log a message as a warn message with an exception
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Warn(object source, object message, Exception exception)
        {
            Warn(source.GetType(), message, exception);
        }

        /// <summary>
        /// log a message as a warn message with an exception
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Warn(Type source, object message, Exception exception)
        {
            getLogger(source).Warn(message, exception);
        }

        /// <summary>
        /// log a message as an error message with an exception
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Error(object source, object message, Exception exception)
        {
            Error(source.GetType(), message, exception);
        }

        /// <summary>
        /// log a message as an error message with an exception
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Error(Type source, object message, Exception exception)
        {
            getLogger(source).Error(message, exception);
        }

        /// <summary>
        /// log a message as a fatal message with an exception
        /// </summary>
        /// <param name="source">object source</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Fatal(object source, object message, Exception exception)
        {
            Fatal(source.GetType(), message, exception);
        }

        /// <summary>
        /// log a message as a fatal message with an exception
        /// </summary>
        /// <param name="source">object type</param>
        /// <param name="message">message to log</param>
        /// <param name="exception">exception to log</param>
        public static void Fatal(Type source, object message, Exception exception)
        {
            getLogger(source).Fatal(message, exception);
        }

        private static void initialize()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(
                                              AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                              "Log4Net.config")));
            logInitialized = true;
        }

        /// <summary>
        /// ensures the logging mechanism is initialized
        /// </summary>
        public static void EnsureInitialized()
        {
            if (!logInitialized)
            {
                initialize();
            }
        }
    }
}