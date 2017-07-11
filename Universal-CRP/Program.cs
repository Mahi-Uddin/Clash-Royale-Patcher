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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[UCRP][INFO]    -> This program is made by Zihad from the Zapps team.");
                Console.WriteLine("[UCRP][INFO]    -> You can find the source at https://github.com/Mahi-Uddin/Universal-CRP/");
                Console.WriteLine("[UCRP][INFO]    -> You can download the executable file at https://bit.ly/GetUCRP/");
                Console.WriteLine("[UCRP][INFO]    -> Please enter the version of your Clash Royale APK.");
                Console.WriteLine("[UCRP][INFO]    -> Ex -> 1.9.0");
                Console.ResetColor();
                string apkVersion;
                apkVersion = Console.ReadLine();
                WebClient client = new WebClient();
                string downloadedKey = client.DownloadString("http://zihad.net78.net/games/clash-royale/keys/" + apkVersion + ".txt");
                string fileName = "libg.so";
                byte[] fileBytes = File.ReadAllBytes(fileName);
                byte[] searchPattern = HexToByteArray(downloadedKey);
                byte[] replacePattern = HexToByteArray("72f1a4a4c48e44da0c42310f800e96624e6dc6a641a9d41c3b5039d8dfadc27e");
                IEnumerable<int> positions = FindPattern(fileBytes, searchPattern);
                if (positions.Count() == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[UCRP][ERROR] Pattern not found.");
                    Console.ResetColor();
                    Console.Read();
                    return;
                }
                string backupFileName = fileName + ".bak";
                File.Copy(fileName, backupFileName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[UCRP][INFO] Backup file: {0} -> {1}", fileName, backupFileName);
                Console.ResetColor();
                foreach (int pos in positions)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[UCRP][INFO] Key offset: 0x{0}", pos.ToString("X8"));
                    Console.ResetColor();
                    using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Open, FileAccess.Write)))
                    {
                        bw.BaseStream.Seek(pos, SeekOrigin.Begin);
                        bw.Write(replacePattern);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[UCRP][INFO] File: {0} patched", fileName);
                    Console.ResetColor();
                }

                Console.Read();
            }
            catch(Exception ex)
            {
                if(ex is WebException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[UCRP][ERROR] Error connecting to the remote server with given informations.");
                    Console.WriteLine("[UCRP][ERROR] Please check your internet connection, APK version and try again later.");
                    Console.WriteLine("[UCRP][ERROR] If the problem persists, contact me at zihadmahiuddin@gmail.com");
                    Console.Read();
                    Console.ResetColor();
                }
                else if(ex is FileNotFoundException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[UCRP][ERROR] The file 'libg.so' could not be found.");
                    Console.WriteLine("[UCRP][ERROR] Please put the 'libg.so' file in the current folder, check for correct APK version and try again later.");
                    Console.WriteLine("[UCRP][ERROR] If the problem persists, contact me at zihadmahiuddin@gmail.com");
                    Console.Read();
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[UCRP][ERROR] An unknown error has occured.\nError: " + ex.ToString());
                    Console.WriteLine("[UCRP][ERROR] If the problem persists, contact me at zihadmahiuddin@gmail.com");
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