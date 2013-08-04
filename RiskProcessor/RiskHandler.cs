using ConnectionProxyDemo.Proxy;
using System;
using log4net;
using ConnectionProxyDemo.ProxyLib;
using System.Collections.Concurrent;

namespace RiskProcessor 
{
    public class RiskHandler : DiagramHandler
    {
        private ILog log = LoggingManager.GetLoggerForClass();

        Char[] diagChar;

        public RiskHandler()
        {
        }

        public Diagram Handle(Diagram diag)
        {
            PrintDiagram(diag);
            return diag;
        }

        private Int32 Char2Int(Char[] charArray)
        {
            Int32 result = 0;
            return result;
        }

        private void PrintDiagram(Diagram diag)
        {
            Char[] charOut = System.Text.Encoding.Default.GetChars(diag.DiagBody);
            System.Console.WriteLine(charOut);
        }
    }
}
