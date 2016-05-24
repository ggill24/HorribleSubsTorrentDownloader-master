using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.PhantomJS;
using System.Linq;
using HorribleSubsTorrentDownloader.Enums;
using System.Text.RegularExpressions;

using HtmlAgilityPack;
using Selenium;
namespace HorribleSubsTorrentDownloader.Classes
{
    class Tracker
    {

        public void CheckForNewEpisodes(Dictionary<int, string> anime, TorrentQuality quality)
        {
            var titles = anime.Values.ToList();
            var episodes = anime.Keys.ToList();
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
                    var trimBefore = downloadLinks.Substring(downloadLinks.IndexOf("href=\"http://www.nyaa.se/?page=download", 0));
                    var index = trimBefore.IndexOf("\">Torrent</a>");
                    if (index < 0) { Console.WriteLine("Could not retrieve torrent link"); continue; }
                    var torrentLink = trimBefore.Substring(0, index);
                    Console.WriteLine(torrentLink);
                }
                Console.ReadKey();
            }
        }
    }
}

                   








