using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace YuoTools.Extend.Helper.Network
{
    public static class NetworkHelper
    {
        public static IPAddress GetLocalAddress()
        {
            // 获取本地计算机上的所有网络接口
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // 过滤掉非活动的网络接口和回环接口
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    // 获取网络接口的IP属性
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    // 获取网络接口的所有IP地址
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        // 过滤掉IPv6地址和非本地地址
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ip.Address))
                        {
                            return ip.Address;
                        }
                    }
                }
            }

            return null;
        }

        public static IPEndPoint ToIPEndPoint(string host, int port)
        {
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public static IPEndPoint ToIPEndPoint(string address)
        {
            var index = address.LastIndexOf(':');
            var host = address.Substring(0, index);
            var p = address.Substring(index + 1);
            var port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }

        private static IPAddress localAddress;
        public static IPAddress LocalAddress => localAddress ??= GetLocalAddress();

        public static IPEndPoint LocalIPEndPoint(int port) => new(LocalAddress, port);

        public static string[] GetAddressIPs()
        {
            List<string> list = new List<string>();
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                {
                    continue;
                }

                foreach (UnicastIPAddressInformation add in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    list.Add(add.Address.ToString());
                }
            }

            return list.ToArray();
        }

        // 优先获取IPV4的地址
        public static IPAddress GetHostAddress(string hostName)
        {
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            IPAddress returnIpAddress = null;
            foreach (IPAddress ipAddress in ipAddresses)
            {
                returnIpAddress = ipAddress;
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress;
                }
            }

            return returnIpAddress;
        }

        public static void SetSioUdpConnReset(Socket socket)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            const uint IOC_IN = 0x80000000;
            const uint IOC_VENDOR = 0x18000000;
            const int SIO_UDP_CONNRESET = unchecked((int)(IOC_IN | IOC_VENDOR | 12));

            socket.IOControl(SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
        }
    }
}