using System;
using StereoKit;

namespace StereoKitPaintTutorial
{
    class Program
    {
        static Color paintColor = Color.White;
        static float paintSize  = 2 * Units.cm2m;
        static Painting activePainting = new Painting();

        static void Main(string[] args)
        {
            if (!StereoKitApp.Initialize("StereoKitPaintTutorial", Runtime.MixedReality))
                Environment.Exit(1);

            while (StereoKitApp.Step(() =>
            {
                activePainting.UpdateInput(Handed.Right, Color.White, 2*Units.cm2m);
                activePainting.Draw();
            }));

            StereoKitApp.Shutdown();
        }
    }
}
