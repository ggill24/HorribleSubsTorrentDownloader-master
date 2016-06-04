using System;
using System.Threading;
using HorribleSubsTorrentDownloader.Classes;
using HorribleSubsTorrentDownloader.Enums;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;




/*
 *Date: 2016-05-14
 *Downloads torrents for animes that are currently airing
 *Program assumes you already have a torrent installed as it will just open the file after it is done downloading
 *Make sure your torrent is set to associate itself with torrent files and have the download path already set
 *Tested and works well with QBitTorrent
 */
namespace HorribleSubsTorrentDownloade
{
    class Program
    {

        /*Credits to: http://stackoverflow.com/questions/3571627/show-hide-the-console-window-of-a-c-sharp-console-application */
        private IntPtr handle = GetConsoleWindow();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

     


        static void Main(string[] args)
        {

            HorribleSubs hs = new HorribleSubs();


            while (true)
            {
                Tasks currentTask = hs.currentTask();

                switch (currentTask)
                {
                    case Tasks.CreateDirectory:
                        ShowWindow(GetConsoleWindow(), SW_SHOW);
                        FileHandler.CreateDirectory(FileHandler.directoryPath);
                        Thread.Sleep(1250);
                        break;
                    case Tasks.GetQualityPref:
                        ShowWindow(GetConsoleWindow(), SW_SHOW);
                        FileHandler.WriteQualityToSettings(Path.Combine(FileHandler.directoryPath, "usersettings.txt"), hs.QualityPerf());
                        Thread.Sleep(1250);
                        break;
                    case Tasks.CreateAnimeList:
                        ShowWindow(GetConsoleWindow(), SW_SHOW);
                        hs.CreateAnimeList();
                        Thread.Sleep(1250);
                        break;
                    case Tasks.TrackAnime:
                        ShowWindow(GetConsoleWindow(), SW_SHOW);
                        hs.TrackAnime();
                        ShowWindow(GetConsoleWindow(), SW_HIDE);
                        Thread.Sleep(3600000);
                        break;
                }
            }
        }
        public static void RestartApplication()
        {
            var assembly = Assembly.GetExecutingAssembly().Location;
            System.Diagnostics.Process.Start(assembly);
        }
    }
}

                        
                        
   
            
       



 





















