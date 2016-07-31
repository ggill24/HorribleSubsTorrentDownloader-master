using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorribleSubsTorrentDownloader.Classes
{
    struct Anime
    {
        public Anime(List<string> title, List<int> episode)
        {
            Title = title;
            Episode = episode;
        }

        public List<string> Title { get; private set; }
        public List<int> Episode { get; private set; }
    }
}
