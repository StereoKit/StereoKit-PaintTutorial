using StereoKit;

namespace StereoKitPaintTutorial
{
    class PaletteMenu
    {
        Pose  menuPose     = new Pose(new Vec3(0, 0, -0.3f), Quat.LookDir(-Vec3.Forward));
        Color paintColor   = Color.White;
        float paintSize    = 2 * Units.cm2m;
        Model paletteModel = Model.FromFile("Palette.glb", Default.ShaderUI);

        public Color PaintColor { get{ return paintColor; } private set{ paintColor = value; } }
        public float PaintSize  { get{ return paintSize;  } private set{ paintSize  = value; } }

        public void Draw()
        {
            UI.AffordanceBegin("PaletteMenu", ref menuPose, paletteModel.Bounds);
            paletteModel.Draw(Matrix.Identity);

            UI.HSliderAt("Size", ref paintSize, 0.001f, 0.02f, 0, new Vec3(6,-1,0) * Units.cm2m, new Vec2(8,2) * Units.cm2m);
            Lines.Add(new Vec3(6, 1, -1) * Units.cm2m, new Vec3(-2,1,-1) * Units.cm2m, paintColor, paintSize);

            if (UI.VolumeAt("White", new Bounds(new Vec3(4, 7, 0) * Units.cm2m, new Vec3(4,4,2) * Units.cm2m)))
                SetColor(Color.White);
            if (UI.VolumeAt("Red",   new Bounds(new Vec3(9, 3, 0) * Units.cm2m, new Vec3(4,4,2) * Units.cm2m)))
                SetColor(new Color(1,0,0));
            if (UI.VolumeAt("Green", new Bounds(new Vec3(9,-3, 0) * Units.cm2m, new Vec3(4,4,2) * Units.cm2m)))
                SetColor(new Color(0,1,0));
            if (UI.VolumeAt("Blue",  new Bounds(new Vec3(3,-6, 0) * Units.cm2m, new Vec3(4,4,2) * Units.cm2m)))
                SetColor(new Color(0,0,1));

            UI.AffordanceEnd();
        }

        void SetColor(Color color)
        {
            paintColor = color;
            Default.MaterialHand["color"] = color;
        }
    }
}
