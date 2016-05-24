using System.Threading;
using HorribleSubsTorrentDownloader.Classes;


/*
 *Date: 2016-05-14
 *Downloads torrents for animes that are currently airing
 *Program assumes you already have a torrent installed as it will just open the file after it is done downloading
 *Make sure your torrent is set to associate tself with torrent files and have the download path already set
 *Tested and works well with QBitTorrent
 */
namespace HorribleSubsTorrentDownloade
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                HorribleSubs hs = new HorribleSubs();
                Thread.Sleep(5000);
            }

        }
    }
}
   
    