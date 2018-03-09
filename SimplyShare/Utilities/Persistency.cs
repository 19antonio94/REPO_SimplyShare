using System;
using System.IO;

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

        public static void saveUserData(LoggedUser user)
        {
            string path = getUserDirectory();
            File.WriteAllText(path + @"\nomeUtente.txt", user.Nome + '\n' + user.Cognome);

        }

        public static LoggedUser loadUserData()
        {

        }
    }
}
   
