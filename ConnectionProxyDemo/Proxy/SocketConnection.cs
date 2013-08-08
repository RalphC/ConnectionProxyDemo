using ConnectionProxyDemo.ProxyLib;
using log4net;
using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;

namespace ConnectionProxyDemo.Proxy
{
    public class SocketConnection
    {
        private static ILog log = LoggingManager.GetLoggerForClass();

        private Boolean passiveConnection;

        private Socket outConnection;

        private Socket inConnection;

        private IPAddress outAddress;

        private Boolean sending = true;

        private Boolean receiving = true;

        private Byte[] sendBuffer;
        
        private Byte[] receiveBuffer;

        private volatile DiagramContainer sendQueue;

        private volatile DiagramContainer receiveQueue;

        private IPEndPoint outEndPoint;

        private IPEndPoint inEndPoint;

        DiagramHandler handler;

        public SocketConnection(String ServerAddress, Int32 ServerPort, Boolean passive, DiagramContainer inContainer, DiagramContainer outContainer, DiagramHandler diagHandler)
        {
            try
            {
                handler = diagHandler;

                sendQueue = outContainer;
                receiveQueue = inContainer;

                outAddress = IPAddress.Parse(ServerAddress);
                outEndPoint = new IPEndPoint(outAddress, ServerPort);
                outConnection = new Socket(outEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                inEndPoint = new IPEndPoint(IPAddress.Loopback, ServerPort);
                inConnection = new Socket(inEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                inConnection.Bind(inEndPoint);

                Thread inThread = new Thread(StartInConnection);
                inThread.Start();

                Thread outThread = new Thread(StartOutConnection);
                outThread.Start(outEndPoint);


            }
            catch (FormatException formatEx)
            {
                log.Error(formatEx.Message);
            }
            catch (SocketException socketEx)
            {
                log.Error(socketEx.Message);
            }
        }

        private void StartPassiveConnection()
        {

        }

        public void SendOutData()
        {
            Diagram toSendDiagram;
            Byte[] toSendByte;
            while (sending)
            {
                toSendDiagram = sendQueue.GetDiagram();
                if (true == toSendDiagram.needSend)
                {
                    toSendByte = toSendDiagram.DiagBody;
                    Monitor.Enter(outConnection);
                    outConnection.Send(toSendByte, SocketFlags.None);
                    Monitor.Exit(outConnection);
                }
            }
        }

        public void ReceiveInData()
        {
            Diagram recDiag;
            Int32 receivedLength;
            while(receiving)
            {
                Monitor.Enter(inConnection);
                receivedLength = inConnection.Receive(receiveBuffer);
                Monitor.Exit(inConnection);
                recDiag = new Diagram(receivedLength, receiveBuffer);
                recDiag = handler.Handle(recDiag);
                if (recDiag.needSend)
                {
                    receiveQueue.AddDiagram(recDiag);
                }
            }
        }

        public void StopReceive()
        {
            receiving = false;
        }

        public void StopSend()
        {
            sending = false;
        }

        public void BreakInConnection()
        {
            inConnection.Close();
        }

        public void BreakOutConnection()
        {
            outConnection.Close();
        }

        public void StartInConnection()
        {
            inConnection.Accept();
            ReceiveInData();
        }

        public void StartOutConnection()
        {
            try
            {
                outConnection.Connect(outEndPoint);
                SendOutData();
            }
            catch (ArgumentNullException nullEx)
            {
                log.Error(nullEx.Message);
            }
            catch (SocketException socketEx)
            {
                log.Error(socketEx.Message);
            }
        }
    }
}
