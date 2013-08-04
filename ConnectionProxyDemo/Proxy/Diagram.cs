using System;

namespace ConnectionProxyDemo.Proxy
{
    public struct Diagram
    {
        public Int32 Length;
        public Boolean needSend;
        public Byte[] DiagBody;

        public Diagram(Int32 length, Byte[] body)
        {
            this.Length = length;
            this.DiagBody = body;
            this.needSend = true;
        }
    }
}
