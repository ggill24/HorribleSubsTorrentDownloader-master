# HorribleSubs Tracker

.Net Framework 4.5.2 or later is required

PhantomJS is required (included in the precompiled version):
http://phantomjs.org/

Torrent client installed (tested and works with qBittorrent) though it should work with any as long as torrent files are associated with your torrent client.

Precompiled version: https://www.mediafire.com/?hiw35kzb6ztopjj

INSTRUCTIONS

1. Run the program once

2. A folder in your C: will be created named "HSTorrentDownloader"

3. Place PhantomJS into that folder

4. Rerun the program and a firewall prompt will appear

5. You must allow as the program uses PhantomJS to retrieve and download the anime episodes

To make changes to your anime list or to add a new anime simply edit the list.txt file located in: C:\HSTorrentDownloader or delete it and restart the program

Format is: [TITLE] [EPISODE]

If you wish to manually add a new anime the spaces must be replaced by dashes ex: shingeki-no-kyojin 1

The program hides itself from view when it is tracking for new episodes, to terminate the program when it is not visible simply open task manager and search for the process: "Horriblesubsdownloader" and right click end task

New episodes are checked once every hour. If you wish to modify it download the source
