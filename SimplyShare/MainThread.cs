using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using shared;
using System.Windows;

//


    //sarebbe bello che mainthread pubblisher 
    //e ricercautente.cs riceve l'evento
/* 
 * Nel pacchetto bisogna inserire il proprio indirizzo IP e la porta in cui si sta ascoltando(8000)-->Fatto con EndPoint
 *
 *
 * 
 * 
 * 
 */

namespace SimplyShare
{
    public class MainThread
    {
        Queue<Packet> queue;
        Dictionary<IPEndPoint, User> utenti;
        Mutex _m = new Mutex(false, "useQ");
        MainWindow windows;
        string testo;
        private delegate void del(string cognome, byte[] immagine);
        IPEndPoint ipep;
        UdpClient newsock;
        User user;
        IPEndPoint sender;
        Thread main_thread;
        Thread anq1;
        Thread anq2;
        Thread invia_file;
        Boolean modalita = true; //true-->publica, false --> privata
        InvioTCP s = null; //
        Thread tcp;
       
        public MainThread(User u)
        {
            main_thread = new Thread(receive_packet);
            utenti = new Dictionary<IPEndPoint, User>();
            anq1 = new Thread(analyze_queue);
            anq2 = new Thread(analyze_queue);
            user = u;
            String IndirizzoIP = null;
            IndirizzoIP = Utilities.Utilities.findIP();
            queue = new Queue<Packet>();
            ipep = new IPEndPoint(IPAddress.Parse(IndirizzoIP),Program.PORTA); //dove ascolto
            newsock = new UdpClient(ipep);  
            sender = new IPEndPoint(IPAddress.Any, Program.PORTA);
            //windows = w;
        }
        public void setListener()
        {
            s = new InvioTCP(ipep, null, user,this); //
            tcp = new Thread(new ThreadStart(s.tcpAscolto));
            tcp.Start();

        }


        public void setUser(User utente)
        {
            user = utente;

        }
        public User getUser()
        {
            return user;
        }
        public void setModalità(Boolean set)
        {
            modalita = set;

        }
        public void p_invia_file(User u,string path,IPEndPoint destinatario) //aggiungere pathfile //u è il destinatario
        {
            

            if (destinatario != null)
            {
                //Send s = new Send(user, path , destinatario, ipep, newsock); 
                //invia_file = new Thread(new ThreadStart(s.r_send_file));
                //invia_file.Start();

                //chiamare tcpConnect qui per mandare il file
                InvioTCP i_file = new InvioTCP(ipep, u, destinatario, this, "invia il file",path);
                invia_file = new Thread(new ThreadStart(i_file.TcpConnect));
                invia_file.Start();
            }
            

        }

        public IPEndPoint getIPfromUser(User u)
        {
            foreach (var l in utenti)
            {
                if (l.Value.nome.Equals(u.nome) && l.Value.cognome.Equals(u.cognome))
                {
                    return l.Key;
                }
            }

            return null;

        }


        public void start_main_thread()
        {
            if (!anq1.IsAlive )
            {
                try
                {
                    anq1.Start();
                    anq2.Start();
                    main_thread.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message+"start threads");
                }               
            }
        }
        private void receive_packet()
        {
            byte[] data = new byte[1024*100];
            //metterci il while attorno
            while (true)
            {
                IPEndPoint ep_s = new IPEndPoint(IPAddress.Any,0);
                data = newsock.Receive(ref ep_s);               
                Packet ricevuto = deserializza(data);
                //inserisco in coda
                _m.WaitOne();
                queue.Enqueue(ricevuto);
                _m.ReleaseMutex();
            }
            //windows.CONTROLLO.Dispatcher.Invoke(delegato1);            
        }
        private void analyze_queue()
        {
            Packet p_estratto;
            del delegato1 = scrivi;
            while (true)
            {
                _m.WaitOne();
                if (queue.Count > 0) //a 
                {
                    p_estratto = queue.Dequeue();
                    //qui eseguo quello che c'e da fare per quel pacchetto
                    if (p_estratto.getDescrizione().Equals( "Annuncio") && modalita == true && !p_estratto.getIpMittente().Equals(ipep))
                    {
                        /*
                         * 
                         * come rispondo :
                         *              
                         *              
                         *              
                         * 
                        */
                        User userNoFoto = new User(user.nome, user.cognome);
                        Packet p_risposta_annuncio = new Packet("Risposta Annuncio",ipep,userNoFoto); //user deve essere senza foto

                        byte[] data_risp_ann = new byte[1024*100];
                        data_risp_ann = serialize(p_risposta_annuncio);                      
                        newsock.Send(data_risp_ann, data_risp_ann.Length, p_estratto.getIpMittente());
                    }
                    if (p_estratto.getDescrizione().Equals("Risposta Annuncio")  && !p_estratto.getIpMittente().Equals(ipep)) //questa porta non va bene
                    {
                       
                        //mettilo nella lista di utenti attivi
                        if (!utenti.ContainsKey(p_estratto.getIpMittente()))
                        {
                            
                            
                            utenti.Add(p_estratto.getIpMittente(), p_estratto.getUser());
                            InvioTCP s1 = new InvioTCP(ipep, p_estratto.getUser(), p_estratto.getIpMittente(),this,"richiedi immagine",null); //qui richiedo l'immagine
                            var tcp = new Thread(new ThreadStart(s1.TcpConnect));
                            tcp.Start();
                        }
                        //windows.lstUtenti.Dispatcher.Invoke(delegato1,new object[] {p_estratto.getUser().getCognome() });      
                        //windows.Dispatcher.Invoke(delegato1, new object[] { p_estratto.getUser().getCognome() }); //+immagine
                    }
                    if (p_estratto.getDescrizione().Equals("Richiesta invio"))
                    {
                        ////message box per sapere se si vuole accettare o no
                        //var result = MessageBox.Show("Accettare i files da " + p_estratto.getUser().nome + " " + p_estratto.getUser().cognome, "Accettare?", MessageBoxButton.YesNo);
                        //if (result == MessageBoxResult.Yes)
                        //{
                        //    Packet p_inviami_il_file = new Packet("", ipep);
                        //}
                        ////problema path_file , come farlo bene ?
                    }
                }
                _m.ReleaseMutex();
            }
        }
        public Dictionary<IPEndPoint, User> getDictionary()
        {
            Thread.Sleep(3000); //un po brutto
            return utenti;
        }
        public void ricerca_utenti()
        {
            utenti.Clear();
            //mando un pacchetto("Annuncio) in broadcast e vedo chi mi risponde
            byte[] data = new byte[1024*100];
            
            Packet p_annuncio = new Packet("Annuncio",ipep); //INSERIRE ANCHE USER OWN ---->user si manda dopo la risposta annuncio
            data = serialize(p_annuncio);
            IPAddress subnet = null;
            try
            {
                subnet = Utilities.Utilities.getSubMask(IPAddress.Parse(Utilities.Utilities.findIP()));
            }
            catch (Exception e) {          
                MessageBox.Show(e.Message + "problema submask");
            }
            IPAddress broadcast = Utilities.Utilities.findBroadCast(IPAddress.Parse(Utilities.Utilities.findIP()), subnet);
            IPEndPoint ip_to = new IPEndPoint(broadcast, Program.PORTA);
            try
            {
                //mando 4 volte per sicurezza
                newsock.Send(data, data.Length, ip_to);
                newsock.Send(data, data.Length, ip_to);
                newsock.Send(data, data.Length, ip_to);
                newsock.Send(data, data.Length, ip_to);
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + ip_to.ToString());
            }
        }
        private void scrivi(string cognome, byte[] immagine_profilo)
        {
            //SE DEVO USARE IL FORM
           // windows.lstUtenti.Items.Add(cognome);
            //windows.txtbox1.Text = testo;           
        }
        public void end_thread()
        {
            try
            {
                anq1.Abort();
                anq2.Abort();
                main_thread.Abort();
            }
            catch
            {
                Console.WriteLine("security exception");
            }
        }
        public void start_thread()
        {
            try
            {
                anq1.Resume();
                anq2.Resume();
                main_thread.Resume();
            }
            catch
            {
                Console.WriteLine("security exception");
            }

        }
        public void svuota_coda()
        {
            //svuotare listbox
           // windows.lstUtenti.Items.Clear();
            utenti.Clear();

        }

        private Packet deserializza(byte[] arrBytes)
        {
            Packet p= null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(arrBytes))
            {              
                p = (Packet)bf.Deserialize(ms);
                return p;
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
                    MessageBox.Show(e.Message+"problema a serializzare");
                }
                return ms.ToArray();
            }
        }
    }
}




