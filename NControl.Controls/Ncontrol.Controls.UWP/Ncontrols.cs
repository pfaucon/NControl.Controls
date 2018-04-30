using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace NControl.Controls.UWP
{
    public class NControls
    {

        /// <summary>
        /// The typefaces.
        /// </summary>
        //public static readonly Dictionary<string, string> tf = new Dictionary<string, string>();
        public static readonly Dictionary<string, FontFamily> Typefaces = new Dictionary<string, FontFamily>();

        /// <summary>
        /// Init this instance to 
        /// </summary>
        public static void Init()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            FontLoader.LoadFonts(assemblies, async (fontName, s) =>
            {
                // must occur first because otherwise the stream can deallocate
                var bytes = new byte[s.Length];
                s.Read(bytes, 0, bytes.Length);

                var fileName = FilenameForFont(fontName);
                var fontString = FontStringForFont(fontName);

                var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var sampleFile = await storageFolder.GetFileAsync(fileName);
                var fileStream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                using (var outputStream = fileStream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        dataWriter.WriteBytes(bytes);
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                fileStream.Dispose(); // Or use the stream variable (see previous code snippet) with a using statement as well.

                var fontFamily = new FontFamily(fontString);
                Typefaces[fontName] = fontFamily;
            });
        }

        public static string FilenameForFont(string fontName)
        {
            return $"{fontName.ToLowerInvariant()}.ttf";
        }
        public static string FontStringForFont(string fontName)
        {
            return $"ms-appdata:///Local/{FilenameForFont(fontName)}#{fontName}";
        }
    }
}
