using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading;
using Selenium;
using System.IO;
using OpenQA.Selenium.PhantomJS;
using HorribleSubsTorrentDownloader.Enums;
using System.Runtime.InteropServices;

namespace HorribleSubsTorrentDownloader.Classes
{
    class HorribleSubs
    {
        //Page containing currently airing animes
        private string currentSeasonHTML = "";
        //Titles of the animes currently airing
        private List<string> animeTitles;
        //animes to track
        private Dictionary<string, int> animes;

        private Tracker animeTracker = new Tracker();



        //Determines what the task to perform
        public Tasks currentTask()
        {
            Tasks t = Tasks.NoTask;
            if (!Directory.Exists(FileHandler.directoryPath)) { return Tasks.CreateDirectory; }
            if (!FileHandler.doesAnimeListExist(FileHandler.directoryPath + "list.txt")) return t = Tasks.CreateAnimeList;
            if (!FileHandler.doesAnimeListExist((Path.Combine(FileHandler.directoryPath + "usersettings.txt")))) return t = Tasks.GetQualityPref;
            t = Tasks.TrackAnime;
            return t;

        }

        public void CreateAnimeList()
        { 
        
          
            Console.WriteLine("Anime list not found");
            currentSeasonHTML = downloadHTMLFile(Dependencies.HSCurrentSeason);

            /*The if-ception begins!*/
            //Retrieve the anime titles
            if (!String.IsNullOrEmpty(currentSeasonHTML))
            {
                animeTitles = GetAnimesAiring();

                //Get animes to follow from user
                if (animeTitles.Count > 0)
                {
                    DisplayAnimeTitles();
                    animes = GetAnimesToFollow();

                    //Write users selection to text file
                    if (animes.Count > 0)
                    {
                        FileHandler.WriteToAnimeList(Path.Combine(FileHandler.directoryPath + "list.txt"), animes);

                    }
                }
            }
        }
       
        public void TrackAnime()
        {
            var list = new Dictionary<string, int>();
            using (StreamReader sr = new StreamReader(Path.Combine(FileHandler.directoryPath, "list.txt")))
            {
                string[] line = null;

                while (sr.Peek() != -1)
                {

                    line = sr.ReadLine().Split(' ');

                    //TODO: Fail Safe Checks Need To Be Added
                    list.Add(line[0], Convert.ToInt32(line[1]));
                }

            }
            animeTracker.CheckForNewEpisodes(list, TorrentQuality.HD);
        }
    
        

        private bool isDigitsAndWhiteSpace(string entry)
        {

            if (entry.All(char.IsNumber) || entry.Any(char.IsWhiteSpace))
            {
                return true;
            }

            return false;
        }
        public TorrentQuality QualityPerf()
        {
            Console.WriteLine("Quality Perference");
            Console.WriteLine("1. " + TorrentQuality.HD + "\n" + "2. " + TorrentQuality.SUBHD + "\n" + "3. " + TorrentQuality.SD);

            int quality = 0;

            while (quality == 0 || quality > 3)
            {
                var userResponse = Console.ReadLine();
                int.TryParse(userResponse, out quality);
                if (quality > 3 || !userResponse.All(char.IsNumber)) { Console.WriteLine("Try again..."); continue; }

            }

            TorrentQuality qualityChosen = TorrentQuality.HD;

            switch (quality)
            {
                case 1:
                    qualityChosen = TorrentQuality.HD;
                    break;
                case 2:
                    qualityChosen = TorrentQuality.SUBHD;
                    break;
                case 3:
                    qualityChosen = TorrentQuality.SD;
                    break;
                default:
                    qualityChosen = TorrentQuality.SUBHD;
                    break;
            }
            return qualityChosen;
        }

        private void DisplayAnimeTitles()
        {

            for (int i = 0; i < animeTitles.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(i.ToString() + ". " + animeTitles[i]);
                Console.WriteLine(sb);
            }
            Console.WriteLine("Animes Found Currently Airing: " + animeTitles.Count);
        }
    
        private Dictionary<string, int> GetAnimesToFollow()
        {
            var animesToFollow = new Dictionary<string, int>();

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Write the anime # you would like to follow along with the episode number to start from seperated by space");
            Console.WriteLine("For more than one anime, seperate by a new line using SPACEBAR");
            Console.WriteLine("Example:");
            Console.WriteLine("0 " + "3" + " Would start tracking: " + animeTitles[0] + " from episode 3 onwards");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Type EXIT when finished");


            while (true)
            {
                //User input with no modification
                string userInputRAW = Console.ReadLine();
                if (userInputRAW.ToLower() == "exit") { break; }


                if (!isDigitsAndWhiteSpace(userInputRAW)) { Console.WriteLine("Numbers only"); continue; }
                //Anime Number & Episode
                string[] userInput = userInputRAW.Split(' ');

                //userInput array should only have a length of 2 since only 2 pieces of information is required (Anime & Episodes
                //No blank
                if (userInput.Length != 2 || String.IsNullOrWhiteSpace(userInput[0]) || String.IsNullOrWhiteSpace(userInput[1])) { Console.WriteLine("Invalid Entry"); continue; }

                int animeTitle = 0;
                int animeNumber = 0;

                if (!int.TryParse(userInput[0], out animeTitle) || !int.TryParse(userInput[1], out animeNumber))  { Console.WriteLine("Invalid Entry"); continue; }

                if (animeTitle > animeTitles.Count - 1) { Console.WriteLine("anime number: " + animeTitle + " does not exist"); continue; }

                if (animesToFollow.Count > 1)
                {

                    if (animesToFollow.ContainsKey(animeTitles[animeTitle])) { Console.WriteLine("You are already following: " + animeTitles[animeTitle]); continue; }
                   
                }
                animesToFollow.Add(animeTitles[animeTitle], animeNumber);




            }
            return animesToFollow;
        }

        private List<string> GetAnimesAiring()
        {
            if (String.IsNullOrEmpty(currentSeasonHTML)) { Console.WriteLine("Could not download HTML page."); return null; }

            Console.Clear();
            HtmlDocument currentSeasonDoc = new HtmlDocument();
            currentSeasonDoc.LoadHtml(currentSeasonHTML);

            List<string> animesTitles = new List<string>();

            try
            {
                //Retrieve the show title and add it to the list
                foreach (var node in currentSeasonDoc.DocumentNode.SelectNodes("//div[@class='ind-show linkful']"))
                {

                    string anime = node.InnerText.Replace(' ', '-');
                    animesTitles.Add(anime);
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Could not find any animes");
                Thread.Sleep(1850);
            }

            return animesTitles;

        }
        private static string downloadHTMLFile(string url)
        {

            string htmlFile = "";
            try
            {
                using (var phantomDriver = new PhantomJSDriver(Dependencies.PhantomJS))
                {
                    Console.Clear();
                    Console.WriteLine("Travelling to: " + url);
                    phantomDriver.Navigate().GoToUrl(url);
                    htmlFile = phantomDriver.PageSource;
                }
            }
            //Sometimes Selenium runs into errors. Though the program runs on a loop of tasks to perform so it is ok to leaves this catch unhandled
            catch (SeleniumException) { }
            catch (OpenQA.Selenium.WebDriverTimeoutException) { }
            catch (OpenQA.Selenium.DriverServiceNotFoundException)
            {
                Console.WriteLine("PhantomJS must be placed in: " + FileHandler.directoryPath);
                Console.WriteLine("You can download PhantJS from: " + "http://phantomjs.org/download.html");
                Console.ReadKey();
            }

            return htmlFile;

        }

    }
}


