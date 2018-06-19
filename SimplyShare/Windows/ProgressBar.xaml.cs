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
        TcpClient client;

        public ProgressBar(String path_file, TcpClient client)
        {
            InitializeComponent();
            //Inizializzazione barra a 0
            Bar.Value = 0;
            this.path_file = path_file;
            this.client = client;

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
                    var stream = client.GetStream();
                    int count = 0;
                    int size = -1;

                    byte[] buff = new byte[4096];
                    while ((size = fs.Read(buff, 0, buff.Count())) > 0 && !backgroundWorker.CancellationPending)
                    {
                        stream.Write(buff, 0, buff.Count());
                        count += size;
                        int percentComplete = (int)(((float)count * 100) / (float)fs.Length);
                        backgroundWorker.ReportProgress(percentComplete);
                    }
                    if(backgroundWorker.CancellationPending)
                        e.Cancel = true;
                }
                catch(Exception exc){
                    MessageBox.Show(exc.Message + "problema in TCPCONNECT");
                }
                finally
                {
                    br.Close();
                    fs.Close();
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
                //percentageLabel.Text = "0";
            }
            if(Bar.Value == 100)
            {
                display("Operazione completata con successo!");
            }

            //Sia che l'operazione sia stata annullata sia che sia finita, cancello zip e cartella e chiudo il form con la barra
            string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            currentPath = System.IO.Path.Combine(currentPath, "User Profile");
            if (File.Exists(currentPath + "\\tempFileZip.zip"))
            {
                File.Delete(currentPath + "\\tempFileZip.zip");
                Utilities.Utilities.DeleteDirectory(currentPath + "\\tempFile1");
            }
            this.Close();

        }

        //DISPLAY MSG BOX
        private void display(String text)
        {
            MessageBox.Show(text);
        }
    }
}
