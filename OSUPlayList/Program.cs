using System;
using System.IO;
using System.Text.RegularExpressions;

namespace OSUPlayList
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("OSUの実行ファイル(OSU!.exe)のパス > ");
            string OSUPath = Console.ReadLine();
            OSUPath = OSUPath.Replace("\"", "");

            if (!File.Exists(OSUPath)) 
            {
                Console.WriteLine("パスの指定が間違っています。");
                Console.ReadLine();
                return;
            }

            string playlist = "#EXTM3U\n";

            OSUPath = Path.GetDirectoryName(OSUPath);
            OSUPath = Path.Combine(OSUPath, "Songs");
            string[] song_path = Directory.GetDirectories(OSUPath);
            for(int i = 0; i < song_path.Length; i++)
            {
                string[] metafile = Directory.GetFiles(song_path[i], "*.osu");
                using (var reder= new StreamReader(metafile[0]))
                {
                    Console.WriteLine(song_path[i]);
                    string line = "";
                    string audio_file = "";
                    string title = "";
                    string artist = "";
                    while ((line = reder.ReadLine()) != null)
                    {
                        if(Regex.IsMatch(line, "^AudioFilename:"))
                        {
                            audio_file = Regex.Replace(line, "^AudioFilename: ", "");
                        }
                        if (Regex.IsMatch(line, "^Title:"))
                        {
                            title = Regex.Replace(line, "^Title:", "");
                        }
                        if (Regex.IsMatch(line, "^TitleUnicode:"))
                        {
                            title = Regex.Replace(line, "^TitleUnicode:", "");
                        }
                        if (Regex.IsMatch(line, "^Artist:"))
                        {
                            artist = Regex.Replace(line, "^Artist:", "");
                        }
                        if (Regex.IsMatch(line, "^ArtistUnicode:"))
                        {
                            artist = Regex.Replace(line, "^ArtistUnicode:", "");
                        }
                    }
                    audio_file = Path.Combine(song_path[i], audio_file);
                    playlist += string.Format("#EXTINF:-1,{0} - {1}\n{2}\n", artist, title, audio_file);
                    TagLib.File file = TagLib.File.Create(audio_file);
                    file.Tag.Title = title;
                    file.Tag.AlbumArtists = new string[] { artist };
                    file.Save();
                    Console.WriteLine(string.Format(">>>Title:{0}\n>>>Artist:{1}", title, artist));
                }
            }
            using (var writer = new StreamWriter("./playlist.m3u",false))
            {
                writer.Write(playlist);
            }
        }
    }
}
