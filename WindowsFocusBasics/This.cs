using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace System
{
    public static class This
    {
        private static readonly string _MyCollectedPath;
        private static List<string> _MyCollected;

        public const string Name = "Windows 聚焦";

        public const string WindowsFocusHandler = nameof(WindowsFocusHandler);

        public static readonly string WindowsFocusDirectory;

        public static readonly string WindowsFocusSaveDirectory;

        public static List<string> MyCollected
        {
            get
            {
                if (null == _MyCollected)
                {
                    var path = _MyCollectedPath;
                    if (File.Exists(path))
                    {
                        var file = File.OpenRead(path);
                        using (file)
                        {
                            var reader = new StreamReader(file);
                            var json = reader.ReadToEnd();
                            _MyCollected = JsonConvert.DeserializeObject<List<string>>(json);
                        }
                    }
                    else
                    {
                        _MyCollected = new List<string>();
                    }
                }
                return _MyCollected;
            }
        }

        static This()
        {
            WindowsFocusDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets");
            WindowsFocusSaveDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Name);
            if (!Directory.Exists(WindowsFocusSaveDirectory))
                Directory.CreateDirectory(WindowsFocusSaveDirectory);
            _MyCollectedPath = Path.Combine(WindowsFocusSaveDirectory, nameof(MyCollected));
        }

        public static void MyCollectedSave()
        {
            var json = JsonConvert.SerializeObject(MyCollected);
            var file = File.Create(_MyCollectedPath);
            using (file)
            {
                var writer = new StreamWriter(file);
                using (writer)
                {
                    writer.Write(json);
                }
            }
        }
    }
}
