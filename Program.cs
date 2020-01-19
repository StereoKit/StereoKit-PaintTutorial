using System;
using StereoKit;

namespace StereoKitPaintTutorial
{
    class Program
    {
        static Painting    activePainting = new Painting();
        static PaletteMenu paletteMenu;
        static Pose        paintingPose = new Pose(Vec3.Zero, Quat.Identity);
        static Pose        menuPose     = new Pose(new Vec3(0, 0, -0.3f), Quat.LookDir(-Vec3.Forward));

        static void Main(string[] args)
        {
            StereoKitApp.settings.assetsFolder = "Assets";
            if (!StereoKitApp.Initialize("StereoKitPaintTutorial", Runtime.MixedReality))
                Environment.Exit(1);

            paletteMenu = new PaletteMenu();

            while (StereoKitApp.Step(() =>
            {
                paletteMenu.Draw();

                UI.AffordanceBegin("PaintingRoot", ref paintingPose, new Bounds(Vec3.One*5*Units.cm2m), true);
                activePainting.UpdateInput(Handed.Right, paletteMenu.PaintColor, paletteMenu.PaintSize);
                activePainting.Draw();
                UI.AffordanceEnd();

                UI.WindowBegin("Menu", ref menuPose, new Vec2(20, 0) * Units.cm2m);
                UI.Button("Save");
                UI.SameLine();
                UI.Button("Load");
                if (UI.Button("Clear"))
                    activePainting = new Painting();
                if (UI.Button("Quit"))
                    StereoKitApp.Quit();
                UI.WindowEnd();
            }));

            StereoKitApp.Shutdown();
        }
    }
}
