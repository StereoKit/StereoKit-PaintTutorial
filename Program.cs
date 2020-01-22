using System;
using System.IO;
using StereoKit;
using StereoKit.Framework;

namespace StereoKitPaintTutorial
{
    class Program
    {
        static Painting    activePainting = new Painting();
        static PaletteMenu paletteMenu;
        static Pose        paintingPose  = new Pose(Vec3.Zero, Quat.Identity);
        static Pose        menuPose      = new Pose(new Vec3(0, 0, -0.3f), Quat.LookDir(-Vec3.Forward));
        static string      defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        static void Main(string[] args)
        {
            StereoKitApp.settings.assetsFolder = "Assets";
            if (!StereoKitApp.Initialize("StereoKitPaintTutorial", Runtime.MixedReality))
                Environment.Exit(1);

            paletteMenu = new PaletteMenu();

            while (StereoKitApp.Step(() =>
            {
                paletteMenu.Draw();
                ShowPainting();
                ShowMenuWindow();
            }));

            StereoKitApp.Shutdown();
        }

        static void ShowPainting()
        {
            UI.AffordanceBegin("PaintingRoot", ref paintingPose, new Bounds(Vec3.One * 5 * Units.cm2m), true);
            activePainting.UpdateInput(Handed.Right, paletteMenu.PaintColor, paletteMenu.PaintSize);
            activePainting.Draw();
            UI.AffordanceEnd();
        }

        static void ShowMenuWindow()
        {
            UI.WindowBegin("Menu", ref menuPose, new Vec2(20, 0) * Units.cm2m);

            if (UI.Button("Save"))
                SavePainting(defaultFolder + "/test.skp");
            /*FilePicker.Show(
                defaultFolder,
                SavePainting,
                new FilePicker.Filter("Painting", "*.skp"));*/

            UI.SameLine();
            if (UI.Button("Load"))
                FilePicker.Show(
                    defaultFolder,
                    LoadPainting,
                    new FilePicker.Filter("Painting", "*.skp"));

            if (UI.Button("Clear"))
                activePainting = new Painting();

            if (UI.Button("Quit"))
                StereoKitApp.Quit();

            UI.WindowEnd();
        }

        static void LoadPainting(string file)
            => activePainting = Painting.FromFile(File.ReadAllText(file));
        
        static void SavePainting(string file)
            => File.WriteAllText(file, activePainting.ToFileData());
        
    }
}
