using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WallpaperManager.Model
{
    public class WallpaperGroup
    {
        [JsonIgnore]
        static readonly string[] Filter = new[] { ".jpg", ".jpeg", ".png", ".bmp" };

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public bool IsRandom { get; set; }
        public TimeSpan Interval { get; set; }

        public List<string>? Displays { get; set; }

        public List<string>? Paths { get; set; }

        [JsonIgnore]
        public IEnumerable<Tuple<string, string>> AllImages
        {
            get
            {
                if (Paths == null)
                    return Enumerable.Empty<Tuple<string, string>>();

                var result = new List<Tuple<string, string>>();
                foreach (var path in Paths)
                {
                    if (File.Exists(path))
                    {
                        result.Add(new Tuple<string, string>(path, path));
                    }
                    else if (Directory.Exists(path))
                    {
                        var files = Directory.EnumerateFiles(path).Where(x => Filter.Any(x.EndsWith)).Select(x=> new Tuple<string, string>(x, path));
                        result.AddRange(files);
                    }
                }

                return result;
            }
        }
    }
}
