using System;
using System.IO;
using System.Windows.Media.Imaging;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using SimplyShare.Models;
using shared;
using Newtonsoft.Json;

namespace SimplyShare.Utilities
{
    class Persistency
    {
        public static string getUserDirectory()
        {
            //Ricavo la directory corrente
            string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            //Genero il path della cartella in cui voglio salvare i dati
            return Path.Combine(currentPath, "User Profile");
        }

        public static string getDownloadDirectory()
        {
            //Ricavo la directory corrente
            string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            //Genero il path della cartella in cui voglio salvare i dati
            return Path.Combine(currentPath, "Download");
        }

        public static void install()
        {
            string newPath = getUserDirectory();

            //Questo if è come se fosse l'installazione automatica al primo avvio
            if (!Directory.Exists(newPath))
            {
                //Se non esiste creo la directory per l'utente e per i download
                Directory.CreateDirectory(newPath);

                //TASTO DESTRO
                //Aggiunta chiavi di sistema. Richiedono autorizzazione (da fare). Apre un'istanza per ogni parametro di ingresso
                //SU CARTELLE 
                RegistryKey key;
                key = Registry.ClassesRoot.CreateSubKey(@"Directory\shell\Condividi con SimpleShare");
                key = Registry.ClassesRoot.CreateSubKey(@"Directory\shell\Condividi con SimpleShare\command");
                key.SetValue("", '"' + System.Reflection.Assembly.GetEntryAssembly().Location + '"' + '"' + "%1" + '"');
                //SU FILE
                key = Registry.ClassesRoot.CreateSubKey(@"*\shell\Condividi con SimpleShare");
                key = Registry.ClassesRoot.CreateSubKey(@"*\shell\Condividi con SimpleShare\command");
                key.SetValue("", '"' + System.Reflection.Assembly.GetEntryAssembly().Location + '"' + '"' + "%1" + '"');

                //Tasto destro con "invia a". Non richiede autorizzazioni, gestisce selezioni multiple ma apre comunque nuova istanza se programma già aperto
                string link = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\SendTo\SimplyShare.lnk";
                WshShell shell = new WshShell();
                IWshShortcut shortcut = shell.CreateShortcut(link) as IWshShortcut;
                shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
                //shortcut...
                shortcut.Save();
            }

            //Controllo se esiste la cartella di default per i download altrimenti la creo
            string pathDownload = getDownloadDirectory();

            if (!Directory.Exists(pathDownload))
                Directory.CreateDirectory(pathDownload);
        }

        public static void saveUserData(LoggedUser user)
        {
            string path = getUserDirectory();

            //Salvo nome, cognome e cartella download in un file .json
            JsonSerializer serializer = new JsonSerializer();
            JsonUser objToSerialize = new JsonUser(user);
            using (StreamWriter sw = new StreamWriter(path + @"\UserData.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objToSerialize);
            }

            //Salvo foto profilo (l'if va tolto se si inserisce una fotoProfilo di default, magari in install)
            if (user.ProfilePic != null)
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
            JsonUser objDeserialized= new JsonUser();

            if (System.IO.File.Exists(path + @"\UserData.json"))
            {
                using (StreamReader file = System.IO.File.OpenText(path + @"\UserData.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    objDeserialized = (JsonUser)serializer.Deserialize(file, typeof(JsonUser));
                }

                BitmapImage pic = null;

                if (System.IO.File.Exists(path + @"\fotoUtente.jpg"))
                {
                    //Non si può fare semplicemente come linea sotto, altrimenti non è possibile modificare immagine una volta caricata
                    //pic = new BitmapImage(new Uri(path + @"\fotoUtente.jpg"));
                    pic = new BitmapImage();
                    pic.BeginInit();
                    pic.CacheOption = BitmapCacheOption.OnLoad;
                    pic.UriSource = new Uri(path + @"\fotoUtente.jpg");
                    pic.EndInit();
                }

                return new LoggedUser(pic, objDeserialized.Nome, objDeserialized.Cognome, true, objDeserialized.PathDownload);
            }

            //Ritorna null se non esiste il file UserData.json
            return null;
        }

        public static void disinstall()
        {
            //Cancella le chiavi di sistema se esistono
            Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\Condividi con SimpleShare", false);
            Registry.ClassesRoot.DeleteSubKeyTree(@"*\shell\Condividi con SimpleShare", false);
            //Questa era della versione vecchia con link sul cestino
            Registry.ClassesRoot.DeleteSubKeyTree(@"Folder\shell\Condividi con SimpleShare", false);

            //Cancella cartelle create da install e file contenuti
            if(Directory.Exists(getUserDirectory()))
                Directory.Delete(getUserDirectory(), true);
            if(Directory.Exists(getDownloadDirectory()))
                Directory.Delete(getDownloadDirectory(), true);
        }
    }
}
