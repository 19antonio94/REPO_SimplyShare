using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Microsoft.Win32;
using shared;
using SimplyShare.CustomControls;
using System.Net;

namespace SimplyShare.Windows
{
    /// <summary>
    /// Logica di interazione per RicercaUtente.xaml
    /// </summary>
    public partial class RicercaUtente : Window
    {
        MainThread mt = null;
        bool[] selectedUsers;
        byte[] file = null;
        List<User> utenti;
        public RicercaUtente(MainThread mt)
        {
            InitializeComponent();
            this.mt = mt;
            //Thread.Sleep(3000); //aspetto le risposte lo metto in getCoda 
            //new Thread(Populate).Start();
            this.Loaded += RicercaUtente_Loaded;
        }

        private void RicercaUtente_Loaded(object sender, RoutedEventArgs e)
        {
            mt.setListener();
            utenti = Ricerca_utenti();
            visualize(utenti);
            
        }
        private List<User> Ricerca_utenti()
        {
            //List<User> UserToBeDisplayed = new List<User>();

            mt.start_main_thread(); //fino a qui funziona
            mt.ricerca_utenti();
            Dictionary<IPEndPoint, User> dicUs = mt.getDictionary();
            List<User> listUtenti = dicUs.Values.ToList<User>();
            return listUtenti;
            //return null;
        }


        private void InviaButton_Click(object sender, RoutedEventArgs e)
        {
            //selezione utente;
            foreach (ConnectedUser cu in UsersContainer.Children)
            {
                if (cu.isSelected)
                {
                    //invia file a cu
                    //trovare ip di cu 
                    mt.p_invia_file(new User(cu.Nome,cu.Cognome),PercorsoFile.Text);
                    string a;
                    a= "";
                }
            }
        }

        private void SfogliaButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ScegliFile = new OpenFileDialog();
            ScegliFile.DefaultExt = "*";
            if (ScegliFile.ShowDialog() == true && ScegliFile.CheckPathExists)
            {//se ho scelto un file
                string filename = ScegliFile.FileName;
                try
                {
                    file = File.ReadAllBytes(filename);
                }
                catch
                {
                    return;
                }

                PercorsoFile.Text = filename;
            }

        }

        private delegate void del_cleaner(UIElementCollection children);
        private void visualize(List<User> lstUtenti)
        {

            foreach (User u in lstUtenti)
            {
                if (u != null)
                {
                    ConnectedUser cu = new ConnectedUser(u.getNome(),u.getCognome(),null,ToImage(u.getImageByte()));                                      
                    cu.Click += Cu_UserClicked;                  
                    UsersContainer.Children.Add(cu);
                }
            }
        }

        private void Cu_UserClicked(object sender, RoutedEventArgs e)
        {
            if (((ConnectedUser)sender).isSelected)
            {
                ((ConnectedUser)sender).isSelected = false;
                ((ConnectedUser)sender).Background = Brushes.Transparent;
                
            }
            else
            {
                ((ConnectedUser)sender).isSelected = true;
                ((ConnectedUser)sender).Background = Brushes.CadetBlue;
            }
        }

        /* async void Populate()
         {
             List<User> UsersToBeDisplayed;
             //mt.ricerca_utenti();
             //UsersToBeDisplayed = mt.GetUsers();
            // del_cleaner cleaner = CleanUsersContainer;

             //Dispatcher.Invoke(cleaner,new object[]{UsersContainer.Children });
             /*
             foreach(User u in UsersToBeDisplayed)
             {
                 ConnectedUser cu = new ConnectedUser();
                 cu.Name = u.getNome();
                 cu.Cognome = u.getCognome();
                 //cu.UserPicSource = (BitmapImage)u.GetProfilePic();
                 UsersContainer.Children.Add(cu);
             }*/


        // }

        void CleanUsersContainer(UIElementCollection children)
        {
            children.Clear();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            //visualize();
            mt.ricerca_utenti();
            Dictionary<IPEndPoint, User> dicUs = mt.getDictionary();
            List<User> listUtenti = dicUs.Values.ToList<User>();
            visualize(listUtenti);

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
