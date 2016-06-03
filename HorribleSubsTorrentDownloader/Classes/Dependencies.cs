using System;


namespace HorribleSubsTorrentDownloader.Classes
{
    class Dependencies
    {
        public static string PhantomJS { get { return FileHandler.directoryPath; } }
        public const string HSCurrentSeason = @"http://horriblesubs.info/current-season/";
    }
}
