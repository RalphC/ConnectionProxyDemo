using System;

namespace ConnectionProxyDemo.Proxy
{
    public interface DiagramHandler
    {
        public Diagram Handle(Diagram diag);
    }
}
