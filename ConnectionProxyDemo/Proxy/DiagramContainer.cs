using System;

namespace ConnectionProxyDemo.Proxy
{
    public interface DiagramContainer
    {
        // add a new diagram into container
        void AddDiagram(Diagram diag);

        // get one diagram from container and remove
        Diagram GetDiagram();

        // empty container
        void Clear();
    }
}
