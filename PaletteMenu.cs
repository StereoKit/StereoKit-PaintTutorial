using StereoKit;

class PaletteMenu
{
	// These models are both used for the palette UI! One is for the root
	// object, and the other is for the paint swatches. We override the
	// shader while loading it to the UI shader, so it has the proximity
	// finger glow! This helps queue in users that it behaves like a UI
	// element.
	Model _model       = Model.FromFile("Palette.glb", Shader.UI);
	Model _swatchModel = Model.FromFile("Paint.glb",   Shader.UI);

	Pose  _pose  = new Pose(-.4f, 0, -0.4f, Quat.LookDir(1,0,1));
	Color _color = Color.White;
	float _size  = 2 * U.cm;

	// These properties are public, so back in Program.cs, we can get access
	// to these values!
	public Color PaintColor { get{ return _color; } private set{ _color = value; } }
	public float PaintSize  { get{ return _size;  } private set{ _size  = value; } }

	public void Step()
	{
		// Begin a handle for the palette menu, this allows users to  grab 
		// the palette and move it around wherever they see fit! We'll use 
		// the palette model's bounds for the size of the handle, and then 
		// we'll draw the palette model at the center of the handle.
		UI.HandleBegin("PaletteMenu", ref _pose, _model.Bounds);
		_model.Draw(Matrix.Identity);

		// Here's a slider for the brushstroke's size! Then we also draw the 
		// brushstroke above it so we have a preview of the size as well.
		UI.HSliderAt("Size", ref _size, 0.001f, 0.02f, 0, V.XYZ(6,-1,0)*U.cm, V.XY(8,2)*U.cm);
		Lines.Add(V.XYZ(6, 1, -1)*U.cm, V.XYZ(-2,1,-1)*U.cm, _color, _size);

		// Then four color swatches for the user can pick from! Feel free to 
		// add a few more here, or switch the colors up! Try Color.HSV to 
		// create some nice colors :) The positions for the swatches were
		// just eyeballed when making the art assets, so no magic formula
		// here.
		Swatch("White", new Vec3(4, 7, 0)*U.cm, Color.White);
		Swatch("Red",   new Vec3(9, 3, 0)*U.cm, new Color(1,0,0));
		Swatch("Green", new Vec3(9,-3, 0)*U.cm, new Color(0,1,0));
		Swatch("Blue",  new Vec3(3,-6, 0)*U.cm, new Color(0,0,1));

		// And end the affordance handle!
		UI.HandleEnd();
	}

	void Swatch(string id, Vec3 at, Color color)
	{
		// Draw a swatch model using the color it represents!
		_swatchModel.Draw(Matrix.T(at), color);

		// If the users interacts with the volume the swatch model is in,
		// then we'll set the active color, as well as the hand material's
		// color to the swatch's color.
		if (UI.VolumeAt(id, new Bounds(at, V.XYZ(4, 4, 2)*U.cm)))
		{
			_color = color;
			Default.MaterialHand[MatParamName.ColorTint] = color;
			// You could also set individual hands to a custom material using
			// Input.HandMaterial
		}
	}
}