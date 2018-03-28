﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using SimplyShare.Models;

namespace SimplyShare.Windows
{
    /// <summary>
    /// Logica di interazione per Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private LoggedUser _lu = null;
        public LoggedUser Result
        {
            get { return _lu; }
            set { _lu = value; }
        }

        Boolean modalita = true; //true=publica, false = private
        BitmapImage pic; //metterne una di deafault che poi cambia
        bool immagineCaricata = false;
        public Registration()
        {
            InitializeComponent();
            Pubblica.IsChecked = true;
            
            //Load user data from disk (if exist)
            LoggedUser previousAccess = Utilities.Persistency.loadUserData();
            if(previousAccess != null)
            {
                Nome.Text = previousAccess.Nome;
                Cognome.Text = previousAccess.Cognome;
                if(previousAccess.ProfilePic != null)
                {
                    ProfilePic.ImageSource = previousAccess.ProfilePic;
                    immagineCaricata = true;
                }
            }

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Result= new LoggedUser(pic, Nome.Text, Cognome.Text, modalita); //mettere in result tutte le cose che servono
            Utilities.Persistency.saveUserData(Result);                     //Salvo dati utente              
            this.DialogResult = true;                                       //modalità
            this.Close();
        }
        private void Privata_Checked(object sender, RoutedEventArgs e)
        {
            Pubblica.IsChecked = false;
            modalita = false;
        }
        private void SfogliaButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ScegliImmagine = new OpenFileDialog();
            ScegliImmagine.DefaultExt = "png";
            if (ScegliImmagine.ShowDialog() == true && ScegliImmagine.CheckPathExists)
            {//se ho scelto un'immagine
                string filename = ScegliImmagine.FileName;
                
                    try
                    {
                        pic = new BitmapImage(new Uri(filename));
                        ProfilePic.ImageSource = pic;
                        immagineCaricata = true;
                    }
                    catch
                    {
                    }                
            }
        }

        private void Pubblica_Checked(object sender, RoutedEventArgs e)
        {
            Privata.IsChecked = false;
            modalita = true;
            //avvia i thread
        }

        private void Nome_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Nome.Text) && !string.IsNullOrWhiteSpace(Cognome.Text))
                StartButton.IsEnabled = true;
            else
                StartButton.IsEnabled = false;
        }

        private void Cognome_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Nome.Text) && !string.IsNullOrWhiteSpace(Cognome.Text))
                StartButton.IsEnabled = true;
            else
                StartButton.IsEnabled = false;
        }
    }
}
