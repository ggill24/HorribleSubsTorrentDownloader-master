using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.PhantomJS;
using System.Linq;
using HorribleSubsTorrentDownloader.Enums;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.Net;

using HtmlAgilityPack;
using Selenium;
namespace HorribleSubsTorrentDownloader.Classes
{
    class Tracker
    {

        public void CheckForNewEpisodes(Dictionary<string, int> anime, TorrentQuality quality)
        {
            var titles = anime.Keys.ToList();
            var episodes = anime.Values.ToList();
            char[] charactersToRemove = new char[] { '!', '?' };

            using (var driver = new PhantomJSDriver(Dependencies.PhantomJS))
            {
                Console.Clear();

                for (int i = 0; i < titles.Count; i++)
                {

                    //Navigate to the show page
                    string animePage = Path.Combine(Dependencies.HSCurrentSeason, titles[i]);
                    driver.Navigate().GoToUrl(animePage);

                    //Save the html page
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);

                    //Search for the episode with the corresponding quality
                    int videoQuality = (int)quality;

                    //Horriblesubs episode number listing less than 10 have a 0 before the number. This will check and add the 0 if the episode is less than 10
                    string episode = episodes[i] < 10 ? "0" + episodes[i].ToString() : episodes[i].ToString();

                    //Create the xPath based on the episode title and episode number
                    string xPath = String.Concat("//div[@class= " + "'" + "release-links " + titles[i].ToLower() + "-" + episode + "-" + videoQuality + "p" + "'" + "]");
                    HtmlNode node = doc.DocumentNode.SelectSingleNode(xPath);

                    if (node == null) { Console.WriteLine("Anime: " + titles[i] + " not found"); continue; }

                    HtmlNodeCollection children = node.ChildNodes;

                    //Get the torrent link
                    var downloadLinks = children[0].InnerHtml;
                    var trimBefore = downloadLinks.Substring(downloadLinks.IndexOf("http://www.nyaa.se/?page=download", 0));
                    var index = trimBefore.IndexOf("\">Torrent</a>");
                    if (index < 0) { Console.WriteLine("Could not retrieve torrent link"); continue; }
                    var torrentLink = trimBefore.Substring(0, index).Replace("amp;", string.Empty);
                    DownloadTorrent(torrentLink);
                }
               
            }
        }
        private void DownloadTorrent(string url)
        {

            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            using(WebClient wc = new WebClient())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ContentDisposition contentDisposition = new ContentDisposition(response.Headers["content-disposition"]);
                    wc.DownloadFile(url, @"C:\Users\" + Environment.UserName + "\\Desktop\\" + "contentDisposition.FileName");
                    
                }
            }
            
              
                
        }
    }
}

                   








