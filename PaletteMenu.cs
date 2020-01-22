using StereoKit;

namespace StereoKitPaintTutorial
{
    class PaletteMenu
    {
        Pose  _pose        = new Pose(new Vec3(-.4f, 0, -0.4f), Quat.LookDir(1,0,1));
        Model _model       = Model.FromFile("Palette.glb", Default.ShaderUI);
        Model _swatchModel = Model.FromFile("Paint.glb",   Default.ShaderUI);
        Color _color       = Color.White;
        float _size        = 2 * Units.cm2m;
        
        public Color PaintColor { get{ return _color; } private set{ _color = value; } }
        public float PaintSize  { get{ return _size;  } private set{ _size  = value; } }

        public void Draw()
        {
            UI.AffordanceBegin("PaletteMenu", ref _pose, _model.Bounds);
            _model.Draw(Matrix.Identity);

            UI.HSliderAt("Size", ref _size, 0.001f, 0.02f, 0, new Vec3(6,-1,0) * Units.cm2m, new Vec2(8,2) * Units.cm2m);
            Lines.Add(new Vec3(6, 1, -1) * Units.cm2m, new Vec3(-2,1,-1) * Units.cm2m, _color, _size);

            Swatch("White", new Vec3(4, 7, 0) * Units.cm2m, Color.White);
            Swatch("Red",   new Vec3(9, 3, 0) * Units.cm2m, new Color(1,0,0));
            Swatch("Green", new Vec3(9,-3, 0) * Units.cm2m, new Color(0,1,0));
            Swatch("Blue",  new Vec3(3,-6, 0) * Units.cm2m, new Color(0,0,1));

            UI.AffordanceEnd();
        }

        void Swatch(string id, Vec3 at, Color color)
        {
            _swatchModel.Draw(Matrix.T(at), color);
            if (UI.VolumeAt(id, new Bounds(at, new Vec3(4, 4, 2) * Units.cm2m)))
            {
                _color = color;
                Default.MaterialHand["color"] = color;
            }
        }
    }
}
