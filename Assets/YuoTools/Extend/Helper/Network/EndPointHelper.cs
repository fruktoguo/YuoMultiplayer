using System.Net;

namespace YuoTools.Extend.Helper
{
    public static class EndPointHelper
    {
        public static IPEndPoint Clone(this EndPoint endPoint)
        {
            IPEndPoint ip = (IPEndPoint)endPoint;
            ip = new IPEndPoint(ip.Address, ip.Port);
            return ip;
        }
    }
    
    public enum NetworkProtocol
    {
        TCP,
        KCP,
        Websocket,
        UDP,
    }
}