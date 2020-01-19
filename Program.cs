using System;
using StereoKit;

namespace StereoKitPaintTutorial
{
    class Program
    {
        static Painting activePainting = new Painting();
        static PaletteMenu paletteMenu;

        static void Main(string[] args)
        {
            Log.Filter = LogLevel.Diagnostic;
            StereoKitApp.settings.assetsFolder = "Assets";
            if (!StereoKitApp.Initialize("StereoKitPaintTutorial", Runtime.MixedReality))
                Environment.Exit(1);

            paletteMenu = new PaletteMenu();

            while (StereoKitApp.Step(() =>
            {
                paletteMenu.Draw();
                activePainting.UpdateInput(Handed.Right, paletteMenu.PaintColor, paletteMenu.PaintSize);
                activePainting.Draw();
            }));

            StereoKitApp.Shutdown();
        }
    }
}
