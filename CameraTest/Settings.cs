using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptureDisplay
{
    public class Settings
    {
        public int RenderMode = 0;
        public string VideoName = "";
        public string AudioName = "";
        public string RenderSizePos = "null";
        public int DisplayModePos = 0;

        public void Save(string path)
        {
            var serializer = JsonConvert.SerializeObject(this);
            string paths = path + "/Settings.json";
            Directory.CreateDirectory(path);
            File.WriteAllText(paths, serializer);
        }

        public static Settings Load(string path)
        {
            string paths = path + "/Settings.json";
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<Settings>(stream);
                return container;
            }
            else
            {
                return new Settings();
            }
        }
    }

}
