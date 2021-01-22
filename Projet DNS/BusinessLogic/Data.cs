using Projet_DNS.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Projet_DNS.BusinessLogic
{
    public static class Data
    {
        #region Propriétées
        private static string DirectoryPath = "";
        public static List<string> TLD;
        public static string querylogfull = "";
        private static string blacklistLog = "";
        private static string whitelistLog = "";
        public static List<QueryLog> querylogs { get; private set; } = new List<QueryLog>();
        public static List<Blacklist> blacklists { get; private set; } = new List<Blacklist>();
        public static List<Whitelist> whitelists { get; private set; } = new List<Whitelist>();
        public static List<Toplist> toplists { get; private set; } = new List<Toplist>();
        #endregion

        #region Méthodes
        public static void checkFileExist()
        {

        }

        public static async void LoadData()
        {
            //Recherche du fichier Log
            DirectoryPath = GetSubDirectories();

            //Top level domains
            TLD = new List<string>(File.ReadAllLines(@"wwwroot\TLD.txt"));

            //Si le fichier n'est pas null ou vide on le lit
            if (!string.IsNullOrEmpty(DirectoryPath))
            {
                querylogfull = readFileLog();
                readBlacklist();
                readWhitelist();
            }

            //Si la blacklists ne sont pas vide alors
            if (!string.IsNullOrEmpty(blacklistLog))
                await SeparateDataBlacklist();

            //Si la Whitelists ne sont pas vide alors
            if (!string.IsNullOrEmpty(whitelistLog))
                await SeparateDataWhitelist();

            //Si les log ne sont pas vide alors
            if (!string.IsNullOrEmpty(querylogfull))
               await SepareDataLog(querylogfull);
        }

        #region Recherche du dossier unbound
        private static string GetSubDirectories()
        {
            string path = "";
            foreach (DriveInfo info in DriveInfo.GetDrives())
            {
                string root = info.Name;

                // Get all subdirectories
                string[] subdirectoryEntries = Directory.GetDirectories(root);

                // Loop through them to see if they have any other subdirectories
                int i = 0;
                for (int j = 0; j < subdirectoryEntries.Length && !LoadSubDirs(subdirectoryEntries[i]).Contains("Unbound"); j++) { i = j; }

                path = LoadSubDirs(subdirectoryEntries[i]);

                if (i >= 0 && i < subdirectoryEntries.Length - 1)
                    break;
            }
            return path;
        }

        private static string LoadSubDirs(string dir)
        {
            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(dir);

                foreach (string subdirectory in subdirectoryEntries)
                {
                    if (subdirectory.Contains("Unbound"))
                    {
                        Console.WriteLine(subdirectory);
                        return subdirectory;
                    }
                    LoadSubDirs(subdirectory);
                }
            }
            catch { }

            return "";
        }
        #endregion

        #region Extraction du fichier log
        private static string readFileLog()
        {
            string pathLog = DirectoryPath + "\\unbound.log";
            // Si le fichier existe
            if (File.Exists(pathLog))
            {
                using (var fs = new FileStream(pathLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        return string.Join(Environment.NewLine, sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Reverse());
                    }
                }
            }

            return "";
        }

        private static async Task SepareDataLog( string entree)
        {
            await Task.Delay(1);
            StringReader sr = new StringReader(entree);

            while (sr.Peek() >= 0)
            {
                string log = sr.ReadLine().ToString();
                string log2 = log.Substring(log.LastIndexOf(']') + 1);

                //Si le log contiens l'adresse local alors on récupère les données
                if (log.Contains("127.0.0.1"))
                {
                    DateTime dateTime = DateTime.Parse(log.Split(' ')[0] + " " + log.Split(' ')[1]);
                    string domain = log2.Split(' ')[3];
                    bool blackTrue = false;
                    string ip = "";
                    try
                    {
                        ip = Dns.GetHostAddresses(domain).FirstOrDefault().ToString();
                    }
                    catch (Exception e)
                    {
                        ip = "-";
                    }

                    if(blacklists.Find(x => x.Domain.Contains(domain) || x.Domain == ".") != null || TLD.Contains(domain))
                    {
                        string ipDomain = Dns.GetHostAddresses(domain).FirstOrDefault().ToString();
                        string ipDomainLists = Dns.GetHostAddresses(blacklists.Find(x => x.Domain.Contains(domain)).Domain).FirstOrDefault().ToString();

                        if (ipDomain.Equals(ipDomainLists))
                            blackTrue = true;

                        for (int i = 0; i < whitelists.Count && blackTrue; i++)
                        {
                            try
                            {
                                ipDomain = Dns.GetHostAddresses(domain).FirstOrDefault().ToString();
                                ipDomainLists = Dns.GetHostAddresses(whitelists[i].Domain).FirstOrDefault().ToString();

                                if (domain.Contains(whitelists[i].Domain) && ipDomain.Equals(ipDomainLists))
                                    blackTrue = false;
                            }
                            catch (Exception e) { }
                        }
                    }

                    QueryLog queryLog = new QueryLog
                    {
                        DateTime = dateTime,
                        Domain = domain,
                        IP = ip,
                        Blacklist = blackTrue
                    };

                    if(!querylogs.Contains(queryLog))
                        //Ajout a la liste
                        querylogs.Add(queryLog);

                    // Ajoute a la liste Toplist ou le hits
                    addTopList(domain);
                }
            }
        }

        public static string returnLog(int NombrLigne = -1)
        {
            if(NombrLigne == -1)
                return string.Join(Environment.NewLine, querylogfull.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            else
                return string.Join(Environment.NewLine, querylogfull.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Take(NombrLigne));
        }

        public static async void refreshDataLog()
        {
            string[] endFile = readFileLog().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] quuerlf = querylogfull.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            int index = Array.IndexOf(endFile, quuerlf[0]);

            //Recupération des dernière données
            string endFile2 = string.Join(Environment.NewLine, endFile.Take(index-1));

            await SepareDataLog(endFile2);

            querylogfull = readFileLog();
        }
        #endregion

        #region TopList
        public static void addTopList(string Domain)
        {
            if (toplists.Find(x => x.Domain == Domain) == null)
                toplists.Add(new Toplist
                {
                    Id = toplists.Count - 1 <= 0 ? 0 : toplists.Count - 1,
                    Domain = Domain,
                    Hits = 1,
                    Frequence = 0,
                    blocked = querylogs.Find(x => x.Domain == Domain).Blacklist
                    
                });
            else
                toplists.Find(x => x.Domain == Domain).Hits++;

            CalculFrequence();
        }

        public static void CalculFrequence()
        {
            foreach(Toplist toplists in toplists.ToList())
            {
                toplists.Frequence = ((toplists.Hits * 100) / Data.toplists.Sum(x => x.Hits));
            }
        } 
        #endregion

        #region Blacklsit
        private static void readBlacklist()
        {
            string pathConf = DirectoryPath + "\\blacklist.conf";
            // Si le fichier existe
            if (File.Exists(pathConf))
            {
                using (var fs = new FileStream(pathConf, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        blacklistLog = string.Join(Environment.NewLine, sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
        }

        private static async Task SeparateDataBlacklist()
        {
            await Task.Delay(1);
            int id = 0;
            string domain = "";
            StringReader sr = new StringReader(blacklistLog);
            
            while (sr.Peek() >= 0)
            {
                string conf = sr.ReadLine().ToString();

                //Si le log contiens l'adresse local alors on récupère les données
                if (conf.Contains("local-zone:"))
                {
                    string pattern = "\".*?\"";
                    Match m = Regex.Match(conf, pattern, RegexOptions.IgnoreCase);

                    if (m.Success)
                        domain = m.Value.Replace("\"", "");

                    //Ajout a la liste
                    blacklists.Add(new Blacklist
                    {
                        Id = id,
                        Domain = domain
                    });
                }
                id++;
            }
        }

        public static bool addBlacklist(string domain)
        {
            string pathConf = DirectoryPath + "\\blacklist.conf";

            //Ajout dans le fichier
            try
            {
                using (StreamWriter w = File.AppendText(pathConf))
                {
                    w.WriteLine(Environment.NewLine);
                    w.WriteLine("# Ajout du domaine : " + domain);
                    w.WriteLine("local-zone: \"" + domain + "\" redirect");
                    w.WriteLine("local-data: \"" + domain + " A 0.0.0.0\"");
                }

                //Ajout du domain dans la blacklist
                blacklists.Add(new Blacklist
                {
                    Id = blacklists.Last().Id + 1,
                    Domain = domain
                });

                //Refresh dns and services
                refreshServeurUnbound();
                refreshDNS();
          
                return true;
            } 
            catch
            {
                return false;
            }
        }

        public static bool deleteBlacklist(string domain)
        {
            string pathConf = DirectoryPath + "\\blacklist.conf";

            try
            {
                //Supression dans le fichier
                File.WriteAllLines(pathConf, File.ReadLines(pathConf).Where(s => !Regex.IsMatch(s, "([\" ])"+@"\b" + domain + @"\b")).ToList());

                //Supression du domain dans la blacklist
                var itemToRemove = blacklists.Single(r => r.Domain == domain);
                blacklists.Remove(itemToRemove);

                //Refresh dns and services
                refreshServeurUnbound();
                refreshDNS();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static async Task refreshBlacklist()
        {
            blacklists.Clear();
            readBlacklist();
            await SeparateDataBlacklist();

            //Refresh dns and services
            refreshServeurUnbound();
            refreshDNS();
        }

        public static void addBlacklistViaAutre(string domain)
        {
            bool deleteWhiteList = false;

            //Supression dans la whitelists
            if (whitelists.Find(x => x.Domain == domain) != null)
            {
                if (deleteWhitelist(domain))
                    deleteWhiteList = true;
            }
            else
                deleteWhiteList = true;

            //Ajout dans la blacklist
            if (deleteWhiteList && whitelists.Find(x => x.Domain == domain) == null)
            {
                if (addBlacklist(domain))
                {
                    foreach (QueryLog queryLog in querylogs.ToList().Where(queryLog => queryLog.Domain == domain))
                    {
                        queryLog.Blacklist = true;
                    }

                    foreach (Toplist toplist in toplists.ToList().Where(toplist => toplist.Domain == domain))
                    {
                        toplist.blocked = true;
                    }
                }
            }
        }
        #endregion

        #region Whitelist
        private static void readWhitelist()
        {
            string pathConf = DirectoryPath + "\\whitelist.conf";
            // Si le fichier existe
            if (File.Exists(pathConf))
            {
                using (var fs = new FileStream(pathConf, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        whitelistLog = string.Join(Environment.NewLine, sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
        }

        private static async Task SeparateDataWhitelist()
        {
            await Task.Delay(1);
            int id = 0;
            string domain = "";
            StringReader sr = new StringReader(whitelistLog);

            while (sr.Peek() >= 0)
            {
                string conf = sr.ReadLine().ToString();

                //Si le log contiens l'adresse local alors on récupère les données
                if (conf.Contains("local-zone:"))
                {
                    string pattern = "\".*?\"";
                    Match m = Regex.Match(conf, pattern, RegexOptions.IgnoreCase);

                    if (m.Success)
                        domain = m.Value.Replace("\"", "");

                    //Ajout a la liste
                    whitelists.Add(new Whitelist
                    {
                        Id = id,
                        Domain = domain
                    });
                }
                id++;
            }
        }

        public static bool addWhitelist(string domain)
        {
            string pathConf = DirectoryPath + "\\whitelist.conf";

            //Ajout dans le fichier
            try
            {
                using (StreamWriter w = File.AppendText(pathConf))
                {
                    w.WriteLine(Environment.NewLine);
                    w.WriteLine("# Ajout du domaine : " + domain);
                    w.WriteLine("local-zone: \"" + domain + "\" transparent");
                }

                //Ajout du domain dans la whitelist
                whitelists.Add(new Whitelist
                {
                    Id = whitelists.Last().Id + 1,
                    Domain = domain
                });

                //Refresh dns and services
                refreshServeurUnbound();
                refreshDNS();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool deleteWhitelist(string domain)
        {
            string pathConf = DirectoryPath + "\\whitelist.conf";

            try
            {
                //Supression dans le fichier
                File.WriteAllLines(pathConf, File.ReadLines(pathConf).Where(s => !Regex.IsMatch(s, "([\" ])" + @"\b" + domain + @"\b")).ToList());

                //Supression du domain dans la blacklist
                var itemToRemove = whitelists.Single(r => r.Domain == domain);
                whitelists.Remove(itemToRemove);

                //Refresh dns and services
                refreshServeurUnbound();
                refreshDNS();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static async Task refreshWhitelist()
        {
            whitelists.Clear();
            readWhitelist();
            await SeparateDataWhitelist();

            //Refresh dns and services
            refreshServeurUnbound();
            refreshDNS();
        }

        public static void addWhitelistViaAutre(string domain)
        {
            bool deleteBlackList = false;

            //Supression dans la blacklists
            if (Data.blacklists.Find(x => x.Domain == domain) != null)
            {
                if (Data.deleteBlacklist(domain))
                    deleteBlackList = true;
            }
            else
                deleteBlackList = true;

            //Ajout dans la whitelists
            if (deleteBlackList && Data.blacklists.Find(x => x.Domain == domain) == null)
            {
                if (Data.addWhitelist(domain))
                {
                    foreach (QueryLog queryLog in Data.querylogs.ToList().Where(queryLog => queryLog.Domain == domain))
                    {
                        queryLog.Blacklist = false;
                    }

                    foreach (Toplist toplist in toplists.ToList().Where(toplist => toplist.Domain == domain))
                    {
                        toplist.blocked = false;
                    }
                } 
            }
        }
        #endregion

        #region Dns and serveur
        public static bool refreshDNS()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.Start();

            p.StandardInput.WriteLine(@"ipconfig /flushdns");
           
            while(p.StandardOutput.Read() != null)
            {
                if(p.StandardOutput.ReadLine().Contains("DNS vidé"))
                    return true;
            }
            return false;
        }

        public static bool refreshServeurUnbound()
        {
            ServiceController service = new ServiceController("unbound");
            try
            {
                if ((service.Status.Equals(ServiceControllerStatus.Running)) || (service.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    service.Stop();
                }
                service.WaitForStatus(ServiceControllerStatus.Stopped);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        #endregion
        #endregion
    }
}