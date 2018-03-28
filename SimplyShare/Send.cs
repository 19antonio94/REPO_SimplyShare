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
using System.Threading;

namespace SimplyShare
{
    class Send
    {
        User u;
        string path;
        IPEndPoint destEP;
        IPEndPoint mioEp;
        UdpClient newsock;
        MainThread mt;
        public Send(User user, String percorso,IPEndPoint epdes, IPEndPoint epmio, UdpClient n)
        {
            u = user;
            path = percorso;
            destEP = epdes;
            mioEp = epmio;
            newsock = n;
        }
        public Send(User user, String percorso, IPEndPoint epdes, IPEndPoint epmio, UdpClient n,MainThread mt)
        {
            u = user;
            path = percorso;
            destEP = epdes;
            mioEp = epmio;
            newsock = n;
            this.mt = mt;
        }

        public void r_send_file()
        {
            Console.WriteLine("path");
            //u deve essere solo nome e cognome
            User n = u;
            n.profilePic = null;
            Packet p_request = new Packet("Richista invio", mioEp, n);
            byte[] p_request_byte = serialize(p_request);
            byte[] data_to_receive = new byte[1024];
            newsock.Send(p_request_byte,p_request_byte.Length,destEP);

            //ricevo la risposta
            data_to_receive = newsock.Receive(ref destEP);
            Packet rispostaImmagine = deserializza(data_to_receive);
            if (rispostaImmagine.getDescrizione().Equals("Accetto file"))
            {
                InvioTCP invioFile = new InvioTCP(mioEp,n,mt,"invia il file");
                
                Thread t = new Thread(new ThreadStart(invioFile.TcpConnect)); //-->"i send image"
                t.Start();

            }
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
        private Packet deserializza(byte[] arrBytes)
        {
            Packet p = null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(arrBytes))
            {
                p = (Packet)bf.Deserialize(ms);
                return p;
            }
        }

    }
}
