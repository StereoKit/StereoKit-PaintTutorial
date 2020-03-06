#if !WINDOWS_UWP
using System;
using System.IO;
using StereoKit.Framework;

namespace StereoKitPaintTutorial
{
    class TextToFromFileWithFilePicker
    {
        static string defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static void Load(Action<string> callback)
        {
            FilePicker.Show(
                FilePickerMode.Open,
                defaultFolder,
                callback,
                new FileFilter("Painting", "*.skp"));
        }

        public static void Save(string fileData)
        {
            Action<string> callback = (string file) =>
            {
                File.WriteAllText(file, fileData);
            };

            FilePicker.Show(
                FilePickerMode.Save,
                defaultFolder,
                callback,
                new FileFilter("Painting", "*.skp"));
        }

    }
}
#endif
