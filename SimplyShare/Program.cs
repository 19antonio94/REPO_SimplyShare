﻿using System;
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
using System.IO;

namespace SimplyShare
{

    class Program
    {
        
        private static TaskbarIcon tbi;
        private static LoggedUser LoggedU;
        static MainThread mt;
        static  RicercaUtente RU;

        [STAThread]
        public static void Main(string[] args)
        {
            mt = null;

            CreateTaskbarIcon();
            install();
            //TaskBarIconThread = new Thread(CreateTaskbarIcon);
            //TaskBarIconThread.SetApartmentState(ApartmentState.STA);
            //TaskBarIconThread.Start();
            ///
            /// Registrazione utente
            ///
            try
            {
                Registration registration = new Registration();

                if (registration.ShowDialog() == false)
                    return;
                LoggedU = (LoggedUser)registration.Result;      //??????
                
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
            ContextMenu cm = new ContextMenu();
            cm.Items.Add(item1);
            cm.Items.Add(item2);          
            tbi.ContextMenu = cm;
  
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

            if (RU.IsInitialized) { RU.Close(); }
            tbi.Dispose();

        }

        private static string getUserDirectory(){
            //Ricavo la directory corrente
            string currentPath = Directory.GetCurrentDirectory();
            //Genero il path della cartella in cui voglio salvare i dati
            return Path.Combine(currentPath, "User Profile");
        }

        private static void install(){
            
            string newPath = getUserDirectory();
            
            //Questo if è come se fosse l'installazione automatica al primo avvio
            if (!Directory.Exists(newPath))
            {
                //Se non esiste creo la directory
                Directory.CreateDirectory(newPath);
            }
        }
    }
}
