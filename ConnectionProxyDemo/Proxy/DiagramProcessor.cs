using System;

namespace ConnectionProxyDemo.Proxy
{
    public interface DiagramHandler
    {
        Diagram Handle(Diagram diag);
    }
}
