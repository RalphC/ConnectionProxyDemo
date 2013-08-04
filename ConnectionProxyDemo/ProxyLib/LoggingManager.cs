using log4net;
using System;
using System.Diagnostics;

namespace ConnectionProxyDemo.ProxyLib
{
    public sealed class LoggingManager
    {
        /**
         * Get the Logger for a class - no argument needed because the calling class
         * name is derived automatically from the call stack.
         *
         * @return Logger
         */
        public static ILog GetLoggerForClass()
        {
            StackTrace stack = new StackTrace();
            String callerName = stack.GetFrame(1).GetMethod().Name;
            return LogManager.GetLogger(callerName);
        }
    }
}
