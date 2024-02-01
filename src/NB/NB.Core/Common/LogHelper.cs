using System;
using Serilog;

namespace NB.Core.Common
{
    public class LogHelper
    {
        public static ILogger Logger { get; set; }


        /// <summary>
        /// Writes an information level logging message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteInfo(object message)
        {
            Logger.Information(message == null ? string.Empty : message.ToString());
        }

        /// <summary>
        /// Writes a warning level logging message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteWarning(object message)
        {
            Logger.Warning(message == null ? string.Empty : message.ToString());
        }

        /// <summary>
        /// Writes a warning level logging message.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        /// <param name="exception">The exception.</param>
        public static void WriteWarning(object message, Exception exception)
        {
            Logger.Warning(exception, message == null ? string.Empty : message.ToString());
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteError(object message)
        {
            Logger.Error(message == null ? string.Empty : message.ToString());
        }

        /// <summary>
        /// Writes the error level logging message..
        /// </summary>
        /// <param name="message">The message to be written.</param>
        /// <param name="exception">The exception.</param>
        public static void WriteError(object message, Exception exception)
        {
            Logger.Error(exception, message == null ? string.Empty : message.ToString());
        }

        /// <summary>
        /// Writes the fatal error level logging message..
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteFatal(object message)
        {
            Logger.Fatal(message == null ? string.Empty : message.ToString());
        }

        /// <summary>
        /// Writes the fatal error level logging message..
        /// </summary>
        /// <param name="message">The message to be written.</param>
        /// <param name="exception">The exception.</param>
        public static void WriteFatal(object message, Exception exception)
        {
            Logger.Fatal(exception, message == null ? string.Empty : message.ToString());
        }



    }
}

