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

namespace SimplyShare
{
    class Program
    {
        private static TaskbarIcon tbi;
        private static LoggedUser LoggedU;
        public static int PORTA = 60020;
        static MainThread mt;
        static  RicercaUtente RU;
        static Registration registration;
        [STAThread]
        public static void Main(string[] args)
        {
            mt = null;
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
            if (registration != null)
            {
                registration.Close();
            }
            if (RU != null)
            {
                RU.Close();
            }
            tbi.Dispose();

        }
    }
}
