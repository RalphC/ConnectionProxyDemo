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

        private Socket outConnection;

        private Socket inConnection;

        private IPAddress outAddress;

        private Boolean sending = true;

        private Boolean receiving = true;

        private Byte[] sendBuffer;
        
        private Byte[] receiveBuffer;

        private volatile ConcurrentQueue<Diagram> sendQueue;

        private volatile ConcurrentQueue<Diagram> receiveQueue;

        //public String Name { get, set,};

        DiagramHandler handler;

        public SocketConnection(String ServerAddress, Int32 ServerPort)
        {
            try
            {
                outAddress = IPAddress.Parse(ServerAddress);
                IPEndPoint ipEP = new IPEndPoint(outAddress, ServerPort);
                outConnection = new Socket(ipEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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

        private void InitSocket()
        {
            sendQueue = new ConcurrentQueue<Diagram>();
            receiveQueue = new ConcurrentQueue<Diagram>();
        }

        public void SendOutData()
        {
            Diagram toSendDiagram;
            Byte[] toSendByte;
            while (sending)
            {
                
                if (sendQueue.TryDequeue(out toSendDiagram))
                {
                    toSendByte = toSendDiagram.DiagBody;
                    Monitor.Enter(outConnection);
                    outConnection.Send(toSendByte, SocketFlags.None);
                    Monitor.Exit(outConnection);
                }
            }
        }

        public void SendInData()
        {
            Diagram toSendDiagram;
            Byte[] toSendByte;
            while (sending)
            {
                if (receiveQueue.TryDequeue(out toSendDiagram))
                {
                    toSendByte = toSendDiagram.DiagBody;
                    Monitor.Enter(inConnection);
                    inConnection.Send(toSendByte, SocketFlags.None);
                    Monitor.Exit(inConnection);
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
                    sendQueue.Enqueue(recDiag);
                }
            }
        }

        public void ReceiveOutData()
        {
            Diagram recDiag;
            Int32 receivedLength;
            while (receiving)
            {
                Monitor.Enter(outConnection);
                receivedLength = outConnection.Receive(receiveBuffer);
                Monitor.Exit(outConnection);
                recDiag = new Diagram(receivedLength, receiveBuffer);
                recDiag = handler.Handle(recDiag);
                if (recDiag.needSend)
                {
                    receiveQueue.Enqueue(recDiag);
                }
            }
        }

        public Int32 CheckReceivedQueue()
        {
            return receiveQueue.Count;
        }

        public Int32 CheckSendQueue()
        {
            return sendQueue.Count;
        }

    }
}
