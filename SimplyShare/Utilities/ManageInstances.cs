using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimplyShare.Utilities
{
    class ManageInstances
    {
        public void ServerPipe()
        {
            Task.Factory.StartNew(() =>
            {

                while (true)
                {
                    var server = new NamedPipeServerStream("SimpleSharePipe", PipeDirection.In);
                    server.WaitForConnection();

                    
                    StreamReader reader = new StreamReader(server);

                    StringBuilder sb = new StringBuilder();
                    sb.Append(reader.ReadToEnd());

                    /*
                     * Per ora il server stampa solo i parametri ricevuti dai client, ma qui si possono aggiungere ad una lista 
                     */
                    string[] stringSeparators = new string[] { "\r\n" };
                   
                    foreach (string s in sb.ToString().Split(stringSeparators, StringSplitOptions.None))
                    {
                        Program.paths.Add(s);
                    }

                    reader.Close();
                    reader.Dispose();
                    server.Close();
                    server.Dispose();
                }
            });
        }

        public void ClientPipe()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            if(arguments.Length >1)
            {

                //Client
                var client = new NamedPipeClientStream(".", "SimpleSharePipe", PipeDirection.Out);
                client.Connect();
                StreamWriter writer = new StreamWriter(client);

                //Manda su ogni riga un args (eccetto args[0] che è il path dell'eseguibile)
                for (int i = 1; i < arguments.Length; i++)
                {
                    writer.WriteLine(arguments[i]);
                }
                writer.Flush();

                //Rilascio risorse
                writer.Close();
                writer.Dispose();
                client.Close();
                client.Dispose();
            }
            
            //Autokill
            Process.GetCurrentProcess().Kill(); /*mutex release before autokilling*/
        }
    }
}
