using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SimplyShare.Models;

namespace SimplyShare.Utilities
{
    class Persistency
    {
        public static string getUserDirectory()
        {
            //Ricavo la directory corrente
            string currentPath = Directory.GetCurrentDirectory();
            //Genero il path della cartella in cui voglio salvare i dati
            return Path.Combine(currentPath, "User Profile");
        }

        public static void install()
        {
            string newPath = getUserDirectory();
        
            //Questo if è come se fosse l'installazione automatica al primo avvio

            if (!Directory.Exists(newPath))
            {
                //Se non esiste creo la directory
                Directory.CreateDirectory(newPath);
                //Aggiungere qui modifiche a chiavi di sistema per tasto destro
            }
        }

        public static void saveUserData(Models.LoggedUser user)
        {
            string path = getUserDirectory();
            //Salvo nome e cognome utente in un file .txt
            File.WriteAllText(path + @"\nomeUtente.txt", user.Nome + Environment.NewLine + user.Cognome);
            //Salvo foto profilo (l'if va tolto se si inserisce una fotoProfilo di default, magari in install)
            if(user.ProfilePic != null)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                String photolocation = path + @"\fotoUtente.jpg";  //file name 
                encoder.Frames.Add(BitmapFrame.Create(user.ProfilePic));
                using (var filestream = new FileStream(photolocation, FileMode.Create))
                    encoder.Save(filestream);
            }
            
        }

        public static LoggedUser loadUserData()
        {
            string path = getUserDirectory();
            string nome, cognome;
            
            if (File.Exists(path + @"\nomeUtente.txt"))
            {
                var lines = File.ReadAllLines(path + @"\nomeUtente.txt");
                nome = lines[0];
                cognome = lines[1];
                BitmapImage pic = null;

                if (File.Exists(path + @"\fotoUtente.jpg"))
                {
                    //Non si può fare semplicemente come linea sotto, altrimenti non è possibile modificare immagine una volta caricata
                    //pic = new BitmapImage(new Uri(path + @"\fotoUtente.jpg"));
                    pic = new BitmapImage();
                    pic.BeginInit();
                    pic.CacheOption = BitmapCacheOption.OnLoad;
                    pic.UriSource = new Uri(path + @"\fotoUtente.jpg");
                    pic.EndInit();
                }

                return new LoggedUser(pic, nome, cognome, true);
            }

            //Ritorna null se non esiste il file nomeUtente.txt
            return null;
        }
    }
}
