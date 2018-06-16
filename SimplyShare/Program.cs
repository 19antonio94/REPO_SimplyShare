using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using System.Threading;
using SimplyShare.Models;
using SimplyShare.Windows;
using System.Diagnostics;
using shared;
using System.Windows.Media.Imaging;
using SimplyShare.Utilities;
using System.Collections.Concurrent;

namespace SimplyShare
{
    class Program
    {
        private static TaskbarIcon tbi;
        public static LoggedUser LoggedU;
        public static int PORTA = 60020;
        static MainThread mt;
        static  RicercaUtente RU = null;
        public static ConcurrentStack<string> paths;

        /*
         * Utilizzo di un mutex mai rilasciato per distingure server pipe (1 solo, cioè il primo che acquista il mutex),
         * da client pipe (tutti gli altri che non riescono ad acquistare il mutex)
        */
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        static Registration registration = null;

        [STAThread]
        public static void Main(string[] args)
        {
            mt = null;

            //Parametri ricevuti
            /*
            string str = "";

            foreach (string s in args)
                str += s + Environment.NewLine;

            MessageBox.Show(str);*/

            //Manage multi instances of the process
            
            ManageInstances m = new ManageInstances();

            if(mutex.WaitOne(TimeSpan.Zero, true))
            {
                paths = new ConcurrentStack<string>();
                if(args.Length > 1)
                {
                    for (int i = 1; i < args.Length; i++)
                        paths.Push(args[i]);
                }
                Thread manageMultiInstances;
                manageMultiInstances = new Thread(new ThreadStart(m.ServerPipe));
                manageMultiInstances.Start();
            }
            else
            {
                m.ClientPipe();
            }

            CreateTaskbarIcon();
            Utilities.Persistency.install();
            try
            {
                registration = new Registration();
                
                if (registration.ShowDialog() == false)
                {
                    Environment.Exit(Environment.ExitCode);
                    return;
                }
                LoggedU = (LoggedUser)registration.Result;                     
                User u = new User(LoggedU.Nome, LoggedU.Cognome, LoggedU.ProfilePic);
                mt = new MainThread(u);
                mt.setModalità(LoggedU.Modality);
                ///
                /// Mainthread
                ///
                
                RU = new RicercaUtente(mt);
                RU.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
#if DEBUG
            return;
#endif
            Environment.Exit(Environment.ExitCode);
            return;

        }



        public static void CreateTaskbarIcon()
        {
            tbi = new TaskbarIcon();
            tbi.Icon = SimplyShare.Properties.Resources.Icon;
            tbi.ToolTipText = "Simply Share";
            tbi.Visibility = Visibility.Visible;
            MenuItem item1 = new MenuItem();
            item1.Header = "Invia File";
            item1.Click += new RoutedEventHandler(send_file);
            MenuItem item2 = new MenuItem();
            item2.Header = "Esci";
            item2.Click += new RoutedEventHandler(exit);
            MenuItem item3 = new MenuItem();
            item3.Header = "Pubblica";
            item3.Click += new RoutedEventHandler(changeToPublicMode);
            MenuItem item4 = new MenuItem();
            item4.Header = "Privata";
            item4.Click += new RoutedEventHandler(changeToPrivateMode);
            ContextMenu cm = new ContextMenu();
            cm.Items.Add(item3);
            cm.Items.Add(item4);
            cm.Items.Add(item1);
            cm.Items.Add(item2);
            tbi.ContextMenu = cm;
            tbi.TrayLeftMouseDown += new RoutedEventHandler(showWindow);


        }

        private static void send_file(object sender,EventArgs e)
        {
            try
            {
                RU.Show();
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception.Message);

            }
            
            
        }
        private static void exit(object sender, EventArgs e)
        {
            if (registration != null)
            {
                registration.closeByUser = false;
                registration.Close();
            }
            if (RU != null)
            {
                RU.closeByUser = false;
                RU.Close();
            }
            tbi.Dispose();

        }

        private static void changeToPublicMode(object sender, EventArgs e)
        {
            if (registration != null)
            {
                if (!registration.modalita)
                {
                    registration.Privata.IsChecked = false;
                    registration.Pubblica.IsChecked = true;
                    registration.modalita = true;
                }
            }

            if (mt != null)
            {
                if(!mt.getModalità())
                    mt.setModalità(true);
            }
        }

        private static void changeToPrivateMode(object sender, EventArgs e)
        {
            if (registration != null)
            {
                if (registration.modalita)
                {
                    registration.Pubblica.IsChecked = false;
                    registration.Privata.IsChecked = true;
                    registration.modalita = false;
                } 
            }

            if (mt != null)
            {
                if (mt.getModalità())
                    mt.setModalità(false);
            }
        }

        private static void showWindow(object sender, RoutedEventArgs e)
        {
            if(registration != null)
            {
                registration.ShowInTaskbar = true;      //Rimette l'icona nella barra applicazioni
                registration.WindowState = WindowState.Normal;      //Rivisualizza la finestra
                registration.Topmost = true;        //Mette la finestra sopra le altre
            }
            if(RU != null)
            {
                RU.ShowInTaskbar = true;
                RU.WindowState = WindowState.Normal;
                RU.Topmost = true;
            }
        }
    }
}
