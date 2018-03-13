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
using System.Windows.Media.Imaging;

namespace SimplyShare
{
    public class InvioTCP
    {
        IPEndPoint remoteEp;
        IPEndPoint localEP;
        User PCuser; //sarebbe il risultato con la foto
        User utente_image; //utente in cui si carica la foto
        Int32 usedPort;
        MainThread mt;
        string task;
        public InvioTCP(IPEndPoint localEP,User u, MainThread t)
        {
            utente_image= u;
            this.localEP = localEP;
            mt = t;

        }
        public InvioTCP(IPEndPoint localEP, User u, MainThread t,string task)
        {
            utente_image = u;
            this.localEP = localEP;
            mt = t;
            this.task = task;

        }
        public InvioTCP(IPEndPoint localEP, User u,User PCuser, MainThread t)
        {
            utente_image = u;
            this.localEP = localEP;
            mt = t;
            this.PCuser = PCuser;

        }
        public InvioTCP(IPEndPoint localEP, User u,IPEndPoint remoteEP, MainThread t)
        {
            utente_image = u;
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
                    Utilities.Utilities u = new Utilities.Utilities(client,PCuser);
                    //ThreadPool.QueueUserWorkItem(ThreadProc, client);   //creare un thread normale altrimenti problemi nella visualizzazione  
                    Thread t = new Thread(new ThreadStart(u.ThreadProc));                                                 //STAthread
                    t.Start();                                  

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
                //MessageBox.Show(client.Connected.ToString() + "connessione ");
                //scrivere quello che si vuole fare...la connessione è avvenuta
                if (task.Equals("invia il file"))
                {
                    // prima la richiesta per far capire che mando un file
                    string message = "invio il file";
                    byte[] rispostaByte = new byte[1024];
                    Byte[] messageByte = Encoding.ASCII.GetBytes(message);
                    var stream = client.GetStream();
                    stream.Write(messageByte,0,messageByte.Length);

                    //aspettare conferma e poi inviare file
                    stream.Read(rispostaByte, 0, rispostaByte.Length);
                    string risposta = Encoding.ASCII.GetString(rispostaByte);
                    if (risposta.Equals("aspetto il file"))
                    {
                            //invia file(zip)
                    }


                }

                else {

                    //richiesta immagine
                    string message = "send me image";
                    Byte[] messageByte = Encoding.ASCII.GetBytes(message);
                    Byte[] buffer = new byte[1024];
                    Byte[] immagine;
                    List<Byte> lstimg = new List<byte>();
                    var stream = client.GetStream();
                    stream.Write(messageByte, 0, messageByte.Length);
                    int n_byte_letti = 1028;
                    //ricezione immagine
                    Thread.Sleep(300);
                    if (stream.CanRead)
                    {
                        while (stream.DataAvailable) //impostare condizione di uscita corretta ??solo in debug funziona 
                        {
                            try
                            {
                                n_byte_letti = stream.Read(buffer, 0, buffer.Length);
                                lstimg.AddRange(buffer);
                                //aggiungere buffer alla bitmap
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message + " tcp connect errore lettura");
                            }
                        }
                        immagine = new byte[lstimg.Count];
                        lstimg.CopyTo(immagine);
                        //inserimento in user della foto: 
                        BitmapImage i = ToImage(immagine);
                        //utente_image.setImage(immagine)   //implementare in user
                        utente_image.profilePic = immagine;
                    }
                }

            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message+"problema in TCPCONNECT");
            }



        }
        //private void ThreadProc(object obj)  
        //{
        //    var client = (TcpClient)obj;
        //    // Do your work here 
        //    Console.WriteLine("Connected");
        //        //Ricevi la richiesta("Request Immagine")


        //        //rispondi con immagine o text = 'NO'
                    
        //}

        public int getUsedPort()
        {
            return usedPort;
        }
        public BitmapImage ToImage(byte[] array)
        {
            if (array == null) return null;
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }



    }
}
