using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SimplyShare
{
    public class InvioTCP
    {
        IPEndPoint remoteEp;
        IPEndPoint localEP;
        User utente_da_riempire; //sarebbe il risultato con la foto
        User utente_da_inviare;
        Int32 usedPort;
        MainThread mt;
        public InvioTCP(IPEndPoint localEP,User u, MainThread t)
        {
            utente_da_inviare = u;
            this.localEP = localEP;
            mt = t;

        }
        public InvioTCP(IPEndPoint localEP, User u,IPEndPoint remoteEP, MainThread t)
        {
            utente_da_inviare = u;
            this.localEP = localEP;
            this.remoteEp = remoteEP;
            
        }

        public void tcpAscolto()
        {
            TcpListener server = null;
            try
            {
                Int32 portTCP = 0;
                IPAddress localIP = localEP.Address;
                server = new TcpListener(localIP,portTCP);
               
                server.Start();
                usedPort = ((IPEndPoint)(server.LocalEndpoint)).Port;
                mt.setlstPort(usedPort);
                Byte[] bytes = new Byte[256];

                while (true)
                {
                    Console.WriteLine("waiting for a connection");
                    TcpClient client = server.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ThreadProc, client);
                    


                }
                

            }
            catch(Exception e )
            {
                MessageBox.Show(e.Message + "problema in tcpAscolto");
            }

        }

        public void TcpConnect()
        {
            try
            {
                //Int32 portTCP = 8001;
                TcpClient client = new TcpClient(); ////buuu
                client.Connect(remoteEp.Address,remoteEp.Port);
                MessageBox.Show(client.Connected.ToString() + "connessione ");
                //scrivere quello che si vuole fare...la connessione è avvenuta
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message+"problema in TCPCONNECT");
            }



        }
        private static void ThreadProc(object obj)
        {
            var client = (TcpClient)obj;
            // Do your work here 
            Console.WriteLine("Connected");
            //chiudere la connessione
        }

        public int getUsedPort()
        {
            return usedPort;
        }



    }
}
