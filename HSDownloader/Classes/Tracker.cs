using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.PhantomJS;
using System.Linq;
using HorribleSubsTorrentDownloader.Enums;
using System.Net.Mime;
using System.Net;
using System.Threading;
using HtmlAgilityPack;


namespace HorribleSubsTorrentDownloader.Classes
{
    class Tracker
    {



        public void CheckForNewEpisodes(Dictionary<string, int> anime, TorrentQuality quality)
        {
            var titles = anime.Keys.ToList();
            var episodes = anime.Values.ToList();

            var episodeDlLinks = new List<string>();
            try
            {
                using (var driver = new PhantomJSDriver(Dependencies.PhantomJS))
                {
                    driver.Manage().Timeouts().SetPageLoadTimeout(new TimeSpan(0, 0, 0, 45));
                    Console.Clear();


                    //i only increments when no episode for the current anime is found
                    //This allows to go through each anime, find all the available episodes and download them at once which then the program can sleep for an hour before checking again
                    //This is to reduce network/cpu usage to a minimum
                    for (int i = 0; i < titles.Count;)
                    {


                        //Navigate to the show page
                        string animePage = Path.Combine(Dependencies.HSCurrentSeason, titles[i]).ToLower();

                        if (!driver.Url.Contains(animePage + "/") || driver.Url.Contains("about:blank")) { driver.Navigate().GoToUrl(animePage); }
                       

                        //Save the html page
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(driver.PageSource);

                        //Search for the episode with the corresponding quality
                        int videoQuality = (int)quality;

                        //Horriblesubs episode number listing less than 10 have a 0 before the number. This will check and add the 0 if the episode is less than 10
                        string episode = episodes[i] < 10 ? "0" + episodes[i].ToString() : episodes[i].ToString();

                        //dashes take place of exclamation marks in the HTML document
                        //So titles with ! need to be replaced with - to be able to find them
                        string title = titles[i].ToLower().Replace("!", "-");

                        //Create the xPath based on the episode title and episode number
                        string xPath = String.Concat("//div[@class= " + "'" + "release-links " + title + "-" + episode + "-" + videoQuality + "p" + "'" + "]");
                        HtmlNode node = doc.DocumentNode.SelectSingleNode(xPath);

                        //Some animes have multiple revisions for episode 1 which are nammed 01v1 01v2 etc...
                        if (node == null)
                        {

                            for (int j = 1; j <= 3; j++)
                            {
                                string episodeVersion = episode + "v" + j.ToString();
                                xPath = String.Concat("//div[@class= " + "'" + "release-links " + title + "-" + episodeVersion + "-" + videoQuality + "p" + "'" + "]");
                                node = doc.DocumentNode.SelectSingleNode(xPath);

                                if (node != null)
                                {
                                    break;
                                }
                            }

                            if (node == null)
                            {
                                Console.WriteLine("Anime: " + title + " not found");
                                i++;
                                continue;
                            }


                        }




                        HtmlNodeCollection children = node.ChildNodes;

                        //Retrieve the torrent link
                        var downloadLinks = children[0].InnerHtml;
                        var trimBefore = downloadLinks.Substring(downloadLinks.IndexOf("http://www.nyaa.se/?page=download", 0));
                        var index = trimBefore.IndexOf("\">Torrent</a>");
                        if (index < 0) { Console.WriteLine("Could not retrieve torrent link"); continue; }
                        var torrentLink = trimBefore.Substring(0, index).Replace("amp;", string.Empty);
                        //episodeDlLinks.Add(torrentLink);

                        //move onto the next episode for the anime
                        if (DownloadTorrent(torrentLink))
                        {
                            Console.WriteLine(title.Replace("-", " ") + " " +  episode.ToString() + " downloaded torrent.");
                            Thread.Sleep(850);
                            episodes[i] += 1;
                            FileHandler.UpdateEpisode(titles, episodes);
                            continue;

                        }

                    }
                }
            }
            catch (Selenium.SeleniumException) { }
            catch (OpenQA.Selenium.WebDriverTimeoutException) { }
            catch (OpenQA.Selenium.DriverServiceNotFoundException)
            {
                Console.WriteLine("PhantomJS must be placed in: " + FileHandler.directoryPath);
                Console.WriteLine("You can download PhantJS from: " + "http://phantomjs.org/download.html");
                Console.ReadLine();
            }
            
        }
            

           
        
        private bool DownloadTorrent(string url)
        {


            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                using (WebClient wc = new WebClient())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Name of the torrent is located in the content disposition
                        ContentDisposition contentDisposition = new ContentDisposition(response.Headers["content-disposition"]);
                        string fileName = Path.Combine(FileHandler.directoryPath, contentDisposition.FileName);
                        wc.DownloadFile(new Uri(url), fileName);
                        System.Diagnostics.Process.Start(fileName);
                        Thread.Sleep(1000);
                        File.Delete(fileName);
                        return true;
                        

                    }
                }
            }
            catch (WebException)
            {
                Console.WriteLine("Unable to download from: " + url);
            }
            return false;
        }
    }
}
        



















