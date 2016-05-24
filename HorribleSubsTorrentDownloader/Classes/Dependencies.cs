using System;


namespace HorribleSubsTorrentDownloader.Classes
{
    class Dependencies
    {
        public static string PhantomJS { get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); } }
        public const string HSCurrentSeason = @"http://horriblesubs.info/current-season/";
    }
}
