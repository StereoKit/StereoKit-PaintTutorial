#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;

namespace StereoKitPaintTutorial
{
    class TextToFromFileWithFilePicker
    {
        static CoreDispatcher uiDispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
        static PickerLocationId pickerLocation = PickerLocationId.DocumentsLibrary;

        public static async void Save(string fileData)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("StereoKit Paint", new List<string>(1) { ".skp" });
            picker.DefaultFileExtension = ".skp";
            picker.SuggestedFileName = "Painting.skp";
            picker.SuggestedStartLocation = pickerLocation;
            await uiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var file = await picker.PickSaveFileAsync();
                if (!file.IsAvailable) return;

                await FileIO.WriteTextAsync(file, fileData);
            });
        }

        public static async void Load(Action<string> callback)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".skp");
            picker.SuggestedStartLocation = pickerLocation;

            await uiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var file = await picker.PickSingleFileAsync();
                if (!file.IsAvailable) return;

                var text = await FileIO.ReadTextAsync(file);
                callback(text);
            });
        }

    }
}
#endif
