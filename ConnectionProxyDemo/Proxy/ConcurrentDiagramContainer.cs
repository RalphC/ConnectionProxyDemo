using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ConnectionProxyDemo.Proxy
{
    public class ConcurrentDiagramContainer : DiagramContainer
    {
        private ConcurrentQueue<Diagram> queue = new ConcurrentQueue<Diagram>();

        public void AddDiagram(Diagram diag)
        {
            queue.Enqueue(diag);
        }

        public Diagram GetDiagram()
        {
            Diagram result = new Diagram();
            result.needSend = false;
            queue.TryDequeue(out result);
            return result;
        }

        public void Clear()
        {
            Monitor.Enter(queue);
            queue = new ConcurrentQueue<Diagram>();
            Monitor.Exit(queue);
        }
    }
}
