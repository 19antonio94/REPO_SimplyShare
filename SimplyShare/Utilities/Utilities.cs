using shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO.Compression;

namespace SimplyShare.Utilities
{
    class Utilities
    {
        TcpClient client;
        User PCuser;

        public Utilities(TcpClient client,User PCuser)
        {
            this.client = client;
            this.PCuser = PCuser;
        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        public void ThreadProc()
        {
            // Do your work here 
            Console.WriteLine("Connected");
            //Ricevi la richiesta("send me image") 
            var stream = client.GetStream();
            byte[] message = MyReceive(stream);
             var right_message = Encoding.ASCII.GetString(message);
            if (Encoding.ASCII.GetString(message).Equals("send me image")) //se mi richiede l'immagine allora la invio
            {
                Int64 size_immagine = PCuser.profilePic.LongLength;              
                byte[] size_b = new byte[8];
                size_b = BitConverter.GetBytes(size_immagine);             
                stream.Write(size_b, 0, size_b.Length);
                stream.Write(PCuser.profilePic, 0, PCuser.profilePic.Length);
                client.Close();             
            }
            if (right_message.Equals("invio il file"))
            {
                var result = MessageBox.Show("Accettare i files" , "Accettare?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    byte[] mess = Encoding.ASCII.GetBytes("aspetto il file");
                    stream.Write(mess, 0, mess.Length);
                    //ricezione e salvataggio file
                    List<Byte> lstBuff = new List<byte>();
                    byte[] tempBuff = new byte[1];
                    byte[] completeMessage;
                    Thread.Sleep(700);
                    while (stream.DataAvailable)
                    {
                        stream.Read(tempBuff, 0, tempBuff.Length);
                        lstBuff.AddRange(tempBuff);
                    }

                    completeMessage = new byte[lstBuff.Count];
                    lstBuff.CopyTo(completeMessage);

                    File.WriteAllBytes(Program.LoggedU.PathDownload+"\\download.zip", completeMessage);
                    try
                    {
                        ZipFile.ExtractToDirectory(Program.LoggedU.PathDownload + "\\download.zip", Program.LoggedU.PathDownload);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Impossibile ricevere il file. Forse l'invio è stato annullato.");
                    }
                    File.Delete(Program.LoggedU.PathDownload + "\\download.zip");
                }
            }
        }

        public static byte[] MyReceive(NetworkStream stream)
        {
            List<Byte> lstBuff = new List<byte>();
            byte[] tempBuff = new byte[1];
            byte[] completeMessage;
            Thread.Sleep(30);
            if (stream.CanRead)
            {
                while (stream.DataAvailable)
                {
                    stream.Read(tempBuff, 0, tempBuff.Length);
                    lstBuff.AddRange(tempBuff);
                }
            }
            completeMessage = new byte[lstBuff.Count];
            lstBuff.CopyTo(completeMessage);
            return completeMessage;
        }


        public static string findIP()
        {
            String nomePc = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry ipMacchina = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (IPAddress ip in ipMacchina.AddressList)
            {
                if (ip.IsIPv6LinkLocal == false && ip.IsIPv6Multicast == false && ip.IsIPv6SiteLocal == false && ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }
        public static IPAddress getSubMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {  
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {   
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
        public static IPAddress findBroadCast(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();
            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
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

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
