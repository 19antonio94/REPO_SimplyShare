using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace SimplyShare
{
    class Send
    {
        User u;
        string path;
        IPEndPoint destEP;
        IPEndPoint mioEp;
        UdpClient newsock;
        public Send(User user, String percorso,IPEndPoint epdes, IPEndPoint epmio, UdpClient n)
        {
            u = user;
            path = percorso;
            destEP = epdes;
            mioEp = epmio;
            newsock = n;
        }

        public void r_send_file()
        {
            Console.WriteLine("path");
            Packet p_request = new Packet("Richista invio", mioEp, u);
            byte[] p_request_byte = serialize(p_request);
            newsock.Send(p_request_byte,p_request_byte.Length,destEP);

        }
        private byte[] serialize(Packet p)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                try
                {
                    bf.Serialize(ms, p);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "problema a serializzare");
                }
                return ms.ToArray();
            }
        }

    }
}
