using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimplyShare.Utilities
{
    class Utilities
    {
        public static string findIP()
        {

            String nomePc = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry ipMacchina = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            //bisogna mettere quello giusto
            foreach (IPAddress ip in ipMacchina.AddressList)
            {
                
                if (ip.IsIPv6LinkLocal == false && ip.IsIPv6Multicast == false && ip.IsIPv6SiteLocal == false && ip.AddressFamily== AddressFamily.InterNetwork)
                {
                   
                    return ip.ToString();
                }

            }

            return null;
          

        }

        public static IPAddress getSubMask(IPAddress address)
        {
        

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));

            

        }


        public static IPAddress findBroadCast(IPAddress address, IPAddress subnetMask)
        {

            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);


        }



    }
}
