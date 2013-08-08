using ConnectionProxyDemo.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RiskProcessor;

namespace ConnectionProxyDemoGUI
{
    public partial class ConnectionProxyDemo : Form
    {
        private DiagramHandler handler;

        ConcurrentDiagramContainer inContainer;

        ConcurrentDiagramContainer outContainer;

        SocketConnection clientSocket;

        SocketConnection serverSocket;

        public ConnectionProxyDemo()
        {
            InitializeComponent();
        }

        private void InitializeProxy()
        {
            handler = new RiskHandler();
            inContainer = new ConcurrentDiagramContainer();
            outContainer = new ConcurrentDiagramContainer();
            clientSocket = new SocketConnection("127.0.0.1", 6666, inContainer, outContainer, handler);
            serverSocket = new SocketConnection("127.0.0.1", 8888, outContainer, inContainer, handler);
        }
    }
}
