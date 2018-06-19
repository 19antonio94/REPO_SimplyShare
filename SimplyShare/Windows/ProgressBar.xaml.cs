using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimplyShare.Windows
{
    /// <summary>
    /// Logica di interazione per ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        private static BackgroundWorker backgroundWorker;
        private String path_file;
        //private IPAddress ip;
        //private int port;
        TcpClient client;

        public ProgressBar(TcpClient c,string p)
        {
            InitializeComponent();
            //Inizializzazione barra a 0
            Bar.Value = 0;
            this.path_file = p;
            //this.ip = ip;
            //this.port = port;
            client = c;

            //Inizializza background worker e associa metodi ad eventi
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //Il backGroundWorker deve partire solo dopo che il form è stato caricato, altrimenti eccezione perchè non si può aggiornare la barra
            Thread.Sleep(50);
            backgroundWorker.RunWorkerAsync();
        }

        private void AnnullaButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (path_file != null)
            {

                    //invia file(zip)
                FileStream fs = new FileStream(path_file, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                try
                {
                    //TcpClient client = new TcpClient();
                    //client.Connect(ip, port);
                    var stream = client.GetStream();
                    int count = 0;
                    int size = -1;

                    byte[] buff = new byte[4096];
                    while ((size = fs.Read(buff, 0, buff.Count())) > 0)
                    {
                        stream.Write(buff, 0, buff.Count());
                        count += size;
                        int percentComplete = (int)(((float)count * 100) / (float)fs.Length);
                        backgroundWorker.ReportProgress(percentComplete);
                    }
                }
                catch(Exception exc){
                    MessageBox.Show(exc.Message + "problema in TCPCONNECT");
                }
                    
            }
        }

        //THIS UPDATES GUI.OUR PROGRESSBAR
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Bar.Value = e.ProgressPercentage;
            //percentageLabel.Text = e.ProgressPercentage.ToString() + " %";
        }

        //WHEN JOB IS DONE THIS IS CALLED.
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                display("Operazione annullata!");
                return;

                //percentageLabel.Text = "0";
            }
            else
            {
                display("Operazione completata con successo!");
                return;
            }
            
        }

        //DISPLAY MSG BOX
        private void display(String text)
        {
            MessageBox.Show(text);
        }
    }
}
