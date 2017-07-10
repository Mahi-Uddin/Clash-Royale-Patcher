using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Universal_CRP
{
    internal class Patcher
    {
        internal static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        internal static string AssemblyVersion
        {
            get
            {
                return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        internal static void Main(string[] args)
        {
            try
            {
                Console.Title = "Universal Clash Royale Patcher " + AssemblyVersion + " - © " + DateTime.Now.Year;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    @"			__________                           
			\____    /____  ______ ______  ______
			  /     /\__  \ \____ \\____ \/  ___/
			 /     /_ / __ \|  |_> >  |_> >___ \ 
			/_______ (____  /   __/|   __/____  >
			        \/    \/|__|   |__|       \/ ");
                Console.ResetColor();
                Console.WriteLine("[UCRP]    -> This program is by Zihad from the Zapps team.");
                Console.WriteLine("[UCRP]    -> You can find the source at https://github.com/Mahi-Uddin/Universal-CRP/");
                Console.WriteLine("[UCRP]    -> Please enter the version of your Clash Royale APK.\nEx -> 1.9.0");
                string apkVersion;
                apkVersion = Console.ReadLine();
                WebClient client = new WebClient();
                string downloadedKey = client.DownloadString("http://zihad.net78.net/games/clash-royale/keys/" + apkVersion + ".txt");
                string fileName = "libg.so";//Place libg in the folder of this program exe

                byte[] fileBytes = File.ReadAllBytes(fileName);
                byte[] searchPattern = HexToByteArray(downloadedKey);


                byte[] replacePattern = HexToByteArray("72f1a4a4c48e44da0c42310f800e96624e6dc6a641a9d41c3b5039d8dfadc27e");//CR Patched

                //Search
                IEnumerable<int> positions = FindPattern(fileBytes, searchPattern);
                if (positions.Count() == 0)
                {
                    Console.WriteLine("[UCRP] Pattern not found.");
                    Console.Read();
                    return;
                }

                //Backup
                string backupFileName = fileName + ".bak";
                File.Copy(fileName, backupFileName);
                Console.WriteLine("[UCRP] Backup file: {0} -> {1}", fileName, backupFileName);

                foreach (int pos in positions)
                {
                    //Replace
                    Console.WriteLine("[UCRP] Key offset: 0x{0}", pos.ToString("X8"));
                    using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Open, FileAccess.Write)))
                    {
                        bw.BaseStream.Seek(pos, SeekOrigin.Begin);
                        bw.Write(replacePattern);
                    }

                    Console.WriteLine("[UCRP] File: {0} patched", fileName);
                }

                Console.Read();
            }
            catch(Exception ex)
            {
                if(ex is WebException)
                {
                    Console.WriteLine("[UCRP] Error connecting to the remote server with given informations.");
                    Console.WriteLine("[UCRP] Please check your internet connection, APK version and try again later.");
                    Console.WriteLine("[UCRP] If the problem persists, contact me at zihadmahiuddin@gmail.com");
                    Console.Read();
                }
                else if(ex is FileNotFoundException)
                {
                    Console.WriteLine("[UCRP] The file 'libg.so' could not be found.");
                    Console.WriteLine("[UCRP] Please put the 'libg.so' file in the current folder, check for correct APK version and try again later.");
                    Console.WriteLine("[UCRP] If the problem persists, contact me at zihadmahiuddin@gmail.com");
                    Console.Read();
                }
                else
                {
                    Console.WriteLine("[UCRP] An unknown error has occured.\nError: " + ex.ToString());
                    Console.WriteLine("[UCRP] If the problem persists, contact me at zihadmahiuddin@gmail.com");
                    Console.Read();
                }
            }
        }
        public static IEnumerable<int> FindPattern(byte[] fileBytes, byte[] searchPattern)
        {
            if ((searchPattern != null) && (fileBytes.Length >= searchPattern.Length))
                for (int i = 0; i < fileBytes.Length - searchPattern.Length + 1; i++)
                    if (!searchPattern.Where((data, index) => !fileBytes[i + index].Equals(data)).Any())
                        yield return i;
        }
    }
}