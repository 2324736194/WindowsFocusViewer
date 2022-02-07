using NLog;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WindowsFocusConsole
{
    public class WindowsFocusHandler : IDisposable
    {
        private readonly IReadOnlyList<ImageFormat> _RawFormats;
        private bool _Handling;
        private readonly ILogger _Logger;
            
        public WindowsFocusHandler()
        {
            _Logger = LogManager.GetCurrentClassLogger();
            _RawFormats = GetImageFormats();
        }
        
        public async void Handle()
        {
            if (_Handling) return;
            _Handling = true;
            var retryDelay = TimeSpan.FromMinutes(30);
            var exceptionDelay = TimeSpan.FromSeconds(30);
            var delay = TimeSpan.FromMinutes(30);
            await Task.Delay(TimeSpan.FromSeconds(1));
            while (_Handling)
            {
                try
                {
                    if (!Directory.Exists(This.WindowsFocusDirectory))
                    {
                        await Task.Delay(retryDelay);
                        continue;
                    }
                    var files = Directory.GetFiles(This.WindowsFocusDirectory);
                    foreach (var file in files)
                    {
                        var stream = default(Stream);
                        try
                        {
                            stream = File.OpenRead(file);
                            var image = Image.FromStream(stream);
                            using (image)
                            {
                                var format = _RawFormats.SingleOrDefault(p => p.Equals(image.RawFormat));
                                if (null != format)
                                {
                                    var fileName = Path.GetFileName(file + "." + format);
                                    var filePath = Path.Combine(This.WindowsFocusSaveDirectory, fileName);

                                    if (!File.Exists(filePath))
                                    {
                                        image.Save(filePath, format);
                                    }
                                }
                            }

                            stream.Close();
                            stream.Dispose();
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Console.WriteLine(ex);
#endif
                            _Logger.Error(ex);
                            if (null != stream)
                            {
                                stream.Close();
                                stream.Dispose();
                            }
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex);
#endif
                    _Logger.Error(ex);
                    await Task.Delay(exceptionDelay);
                    continue;
                }

                await Task.Delay(delay);
            }
        }

        private string GetDateDirectory()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            var directory = Path.Combine(This.WindowsFocusSaveDirectory, date);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        private IReadOnlyList<ImageFormat> GetImageFormats()
        {
            var ownerType = typeof(ImageFormat);
            var properties = ownerType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            var formats = properties
                .Select(p => p.GetValue(null))
                .OfType<ImageFormat>().ToList();
            return formats;
        }

        public void Dispose()
        {
            _Handling = false;
        }
    }
}