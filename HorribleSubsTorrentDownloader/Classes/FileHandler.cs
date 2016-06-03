using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using HorribleSubsTorrentDownloader.Enums;

namespace HorribleSubsTorrentDownloader.Classes
{
    public static class FileHandler
    {
        public const string directoryPath = @"C:\HSTorrentDownloader\";

        public static bool doesAnimeListExist(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }
        public static void WriteToAnimeList(string path, Dictionary<string, int> Anime)
        {
            try
            {
                if (!doesAnimeListExist(Path.Combine(directoryPath + "list.txt")))
                {

                    var file = File.Create(Path.Combine(directoryPath, "list.txt"));
                    file.Close();
                }


                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (var a in Anime)
                    {
                        sw.WriteLine(a.Key + " " + a.Value);
                    }
                }
            }
            catch (FileNotFoundException fileNotFoundEx)
            {
                Console.WriteLine("Error: Could not locate text file to write" + "\n" + fileNotFoundEx.Message);
                Thread.Sleep(5000);
            }
            catch (DirectoryNotFoundException diNotFound)
            {
                Console.WriteLine("Error: Directory: " + directoryPath + "does not exist" + "\n" + diNotFound.Message);
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(5000);
            }
        }
        public static void WriteQualityToSettings(string path, TorrentQuality quality)
        {
            if (!File.Exists(Path.Combine(directoryPath + "usersettings.txt")))
            {
                var fs = File.Create(Path.Combine(directoryPath, "usersettings.txt"));
                fs.Close();
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("Quality: " + (int)quality);
                }
            }
            catch (FileNotFoundException fileNotFoundEx)
            {
                Console.WriteLine("Could not locate text file to write" + "\n" + fileNotFoundEx.Message);
                Thread.Sleep(5000);
            }
            catch (DirectoryNotFoundException diNotFound)
            {
                Console.WriteLine("Directory: " + directoryPath + "does not exist" + "\n" + diNotFound.Message);
                Thread.Sleep(5000);
            }
            
        }
        public static void UpdateEpisode(List<string> anime, List<int> Nextepisode)
        {
            List<string> animeList = new List<string>();
            try
            {
                for (int i = 0; i < Nextepisode.Count; i++)
                {
                    int episode = Nextepisode[i];
                    animeList.Add(String.Concat(anime[i], " ", episode.ToString()));
                }
                if (File.Exists(Path.Combine(directoryPath, "list.txt"))) { File.Delete((Path.Combine(directoryPath, "list.txt"))); }
                File.WriteAllLines(Path.Combine(directoryPath, "list.txt"), animeList.ToArray());

            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Anime list is missing....creating new one");
                var fs = File.Create(Path.Combine(directoryPath));
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine(path + " already exists");
                return;
            }

        }
    }
}











