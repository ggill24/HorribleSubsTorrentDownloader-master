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


namespace HorribleSubsTorrentDownloader.Classes
{
    class HorribleSubs
    {
        //Page containing currently airing animes
        private string currentSeasonHTML = "";
        //Titles of the animes currently airing
        private List<string> animeTitles;
        //animes to track
        public Dictionary<string, int> animes;

        public Tracker animeTracker = new Tracker();

        //Determines what the task to perform
        private Tasks currentTask()
        {
            Tasks t = Tasks.NoTask;
            if (!FileHandler.doesAnimeListExist(FileHandler.directoryPath + "list.txt")) return t = Tasks.CreateAnimeList;
            if (!FileHandler.doesAnimeListExist((Path.Combine(FileHandler.directoryPath + "usersettings.txt")))) return t = Tasks.GetQualityPref;
            t = Tasks.TrackAnime;
            return t;

        }

        public HorribleSubs()
        {
            switch (currentTask())
            {
                case Tasks.CreateAnimeList:
                    Console.WriteLine("Anime list not found!");
                    Thread.Sleep(500);
                    Console.WriteLine("Welcome, " + Environment.UserName + "." + " Let me go grab the current animes airing so that you can make your selection.");
                    //Thread.Sleep(4500);
                    //Download the HTML page containing the animes airing
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
                    break;

                case Tasks.GetQualityPref:
                    FileHandler.WriteQualityToSettings(Path.Combine(FileHandler.directoryPath + "usersettings.txt"), getQualityPerf());
                    break;

                case Tasks.TrackAnime:
                    var list = new Dictionary<string, int>();
                    using (StreamReader sr = new StreamReader(Path.Combine(FileHandler.directoryPath, "list.txt")))
                    {
                        string[] line = null;
                        while (sr.Peek() != -1)
                        {
                           
                            line =  sr.ReadLine().Split(' ');

                            //TODO: Fail Safe Checks Need To Be Added
                            list.Add(line[0], Convert.ToInt32(line[1]));
                        }
                        
                    }
                    
                        
                    animeTracker.CheckForNewEpisodes(list, TorrentQuality.HD);
                    break;

                case Tasks.NoTask:
                    Console.Clear();
                    Console.WriteLine("No task to perform");
                    Console.WriteLine("Check your 'C' drive to see if you have a HSTorrentDownloader folder containing 2 text files that ARE NOT EMPTY." + "\n" + "Try deleting either of the files to trigger a task");
                    break;
            }
        }

        private bool isDigitsAndWhiteSpace(string entry)
        {

            if (entry.All(char.IsNumber) || entry.Any(char.IsWhiteSpace))
            {
                return true;
            }

            return false;
        }
        private TorrentQuality getQualityPerf()
        {
            Console.WriteLine("Quality Perference");
            Console.WriteLine("1. " + TorrentQuality.HD + "\n" + "2. " + TorrentQuality.SUBHD + "\n" + "3. " + TorrentQuality.SUBHD);

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
            try
            {
                for (int i = 0; i < animeTitles.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(i.ToString() + ". " + animeTitles[i]);
                    Console.WriteLine(sb);
                }
                Console.WriteLine("Animes Found Currently Airing: " + animeTitles.Count);
            }
            catch (IndexOutOfRangeException outOfRangeEx)
            {
                Console.WriteLine(outOfRangeEx.Message);
            }

        }

        private Dictionary<string, int> GetAnimesToFollow()
        {
            var animesToFollow = new Dictionary<string, int>();

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Write the anime # you would like to follow along with the episode number to start from seperated by space");
            Console.WriteLine("For more than one anime, seperate by a new line using SPACEBAR");
            Console.WriteLine("Example:");
            Console.WriteLine("3 " + "0" + " Would start tracking: " + animeTitles[0] + " from episode 3 onwards");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Type EXIT when finished");


            while (true)
            {
                //User input with no modification
                string userInputRAW = Console.ReadLine();
                if (userInputRAW == "exit") { break; }


                if (!isDigitsAndWhiteSpace(userInputRAW)) { Console.WriteLine("Numbers only"); continue; }
                //Anime Number & Episode
                string[] userInput = userInputRAW.Split(' ');


                int animeNumber = 0;
                if (!int.TryParse(userInput[1], out animeNumber)) { Console.WriteLine("Invalid Entry"); continue; }

                //userInput array should only have a length of 2 since only 2 pieces of information is required (Anime & Episodes
                //No blank
                if (userInput.Length != 2 || String.IsNullOrWhiteSpace(userInput[0]) || String.IsNullOrWhiteSpace(userInput[1]) || animeNumber > animeTitles.Count - 1) { Console.WriteLine("Invalid Entry"); continue; }

                if (animesToFollow.ContainsKey(animeTitles[Convert.ToInt32(userInput[0])])) { Console.WriteLine("You are already following: " + animeTitles[animeNumber]); continue; }
                var toFollow = animeTitles[Convert.ToInt32(userInput[0])];
                var episodeToFollow = animeNumber;

                animesToFollow.Add(animeTitles[Convert.ToInt32(userInput[0])], animeNumber);


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
                Thread.Sleep(2000);
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
                    Console.WriteLine("Travelling to: " + Dependencies.HSCurrentSeason);
                    phantomDriver.Navigate().GoToUrl(Dependencies.HSCurrentSeason);
                    htmlFile = phantomDriver.PageSource;
                }
            }
            catch (SeleniumException SEexception)
            {
                Console.WriteLine(SEexception.Message);
                Thread.Sleep(5000);
            }

            return htmlFile;

        }

    }
}
