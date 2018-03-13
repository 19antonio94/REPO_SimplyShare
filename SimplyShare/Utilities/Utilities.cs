using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SimplyShare.Utilities
{
    class Utilities
    {
        TcpClient client;
        User PCuser;

        public Utilities(TcpClient client,User PCuser)
        {
            this.client = client;
            this.PCuser = PCuser;
        }

        public void ThreadProc()
        {
            // Do your work here 
            Console.WriteLine("Connected");
            //Ricevi la richiesta("send me image") 
            var stream = client.GetStream();
            byte[] message = MyReceive(stream);
            Encoding.ASCII.GetString(message);
            if (message.Equals("send me image"))
            {
                stream.Write(PCuser.profilePic, 0, PCuser.profilePic.Length);
            }
            if (message.Equals("invio il file"))
            {
                byte[] mess = Encoding.ASCII.GetBytes("aspetto il file");
                stream.Write(mess,0,mess.Length);

                //ricezione e salvataggio file
            }
        }

        private byte[] MyReceive(NetworkStream stream)
        {
            List<Byte> lstBuff = new List<byte>();
            byte[] tempBuff = new byte[1024];
            byte[] completeMessage;
            Thread.Sleep(30);
            if (stream.CanRead)
            {
                while (stream.DataAvailable)
                {
                    stream.Read(tempBuff, 0, tempBuff.Length);
                    lstBuff.AddRange(tempBuff);
                }
            }
            completeMessage = new byte[lstBuff.Count];
            lstBuff.CopyTo(completeMessage);
            return completeMessage;

        }


        public static string findIP()
        {
            String nomePc = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry ipMacchina = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (IPAddress ip in ipMacchina.AddressList)
            {
                if (ip.IsIPv6LinkLocal == false && ip.IsIPv6Multicast == false && ip.IsIPv6SiteLocal == false && ip.AddressFamily == AddressFamily.InterNetwork)
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
