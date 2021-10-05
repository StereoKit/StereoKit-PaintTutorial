using StereoKit;

class PaletteMenu
{
	// These models are both used for the UI! One is a bottle that's used for
	// decoration and context, and will display the active color, and the
	// other is a "splash of ink" that we'll turn into a pressable button,
	// and allow users to pick colors with it.
	Model _model       = Model.FromFile("InkBottle.glb");
	Model _swatchModel = Model.FromFile("InkSplat.glb");

	Pose  _pose  = new Pose(-.4f, 0, -0.4f, Quat.LookDir(1,0,1));
	Color _color = Color.White;
	float _size  = 2 * U.cm;

	float _hue = 0;
	float _saturation = 0;
	float _value = 1;

	// These properties are public, so back in Program.cs, we can get access
	// to these values!
	public Color PaintColor { get{ return _color; } private set{ _color = value; } }
	public float PaintSize  { get{ return _size;  } private set{ _size  = value; } }

	public void Step()
	{
		// We'll use a standard window to hold all of our ink brush settings!
		UI.WindowBegin("Ink", ref _pose);

		// Display an ink bottle to highlight the purpose of this area! The
		// size provided here will auto-fill on the x-axis, which will center
		// the Model, and we'll make it two lines tall.
		UI.Model(_model, V.XY(0, UI.LineHeight*2));

		// Display a list of pre-picked color swatches. These color swatches
		// are implemented as custom buttons, so check out the SwatchColor
		// method below.
		SwatchColor("White", _hue, 0,   1);
		UI.SameLine();
		SwatchColor("Gray",  _hue, 0, .6f);
		UI.SameLine();
		SwatchColor("Blk",   _hue, 0, SK.System.displayType == Display.Additive ? 0.25f : 0);
		UI.SameLine();
		SwatchColor("Green", .33f, .9f, 1);
		UI.SameLine();
		SwatchColor("Ylw",   .14f, .9f, 1);
		UI.SameLine();
		SwatchColor("Red",   0,    .9f, 1);

		UI.Space(UI.LineHeight*0.5f);

		// Swatches are never enough by themselves! So here's some sliders to
		// let the user HSV their color manually. We start with a fixed size
		// label, and on the same line add a fixed size slider. Fixing the
		// sizes here helps them to line up in columns.
		UI.Label("Hue", V.XY(8*U.cm, UI.LineHeight));
		UI.SameLine();
		if (UI.HSlider("Hue", ref _hue, 0, 1, 0, 22*U.cm, UIConfirm.Pinch))
			SetColor(_hue, _saturation, _value);

		UI.Label("Saturation", V.XY(8*U.cm, UI.LineHeight));
		UI.SameLine();
		if (UI.HSlider("Saturation", ref _saturation, 0, 1, 0, 22*U.cm, UIConfirm.Pinch))
			SetColor(_hue, _saturation, _value);

		UI.Label("Value", V.XY(8*U.cm, UI.LineHeight));
		UI.SameLine();
		if (UI.HSlider("Value", ref _value, 0, 1, 0, 22*U.cm, UIConfirm.Pinch))
			SetColor(_hue, _saturation, _value);

		UI.HSeparator();

		// Now for brush sizes! We'll have some size swatches first, these
		// are similar to the color swatches, except they have some control
		// over how large the swatch looks.

		// Reserve some empty space first, so content lines up in columns!
		UI.LayoutReserve(V.XY(8*U.cm,0));

		UI.SameLine();
		if (SwatchSize("Small", 1*U.cm)) _size = 1 * U.cm;
		UI.SameLine();
		if (SwatchSize("Med",   2*U.cm)) _size = 2 * U.cm;
		UI.SameLine();
		if (SwatchSize("Lrg",   3*U.cm)) _size = 3 * U.cm;
		UI.SameLine();
		if (SwatchSize("Xtra",  4*U.cm)) _size = 4 * U.cm;

		UI.Label("Size", V.XY(8 * U.cm, UI.LineHeight));
		UI.SameLine();
		UI.HSlider("Size", ref _size, 0.001f, 0.05f, 0, 22 * U.cm, UIConfirm.Pinch);

		// Display a preview of the brush stroke's size. We'll reserve a box
		// that can hold the maximum size for the brush stroke, and preview
		// the stroke with an unlit cube scaled to the brush's size.
		Bounds linePreview = UI.LayoutReserve(V.XY(0, 0.05f));
		linePreview.dimensions.y = _size;
		linePreview.dimensions.z = U.cm;
		Mesh.Cube.Draw(Material.Unlit, Matrix.TS(linePreview.center, linePreview.dimensions), _color);

		// And end the window!
		UI.WindowEnd();
	}

	void SwatchColor(string id, float hue, float saturation, float value)
	{
		// Reserve a spot for this swatch!
		Bounds bounds = UI.LayoutReserve(_swatchModel.Bounds.dimensions.XY);
		bounds.dimensions.z = U.cm*4;

		// Draw the swatch model using the color it represents! We'll also
		// add some pseudo-random rotation to prevent it from looking too
		// repetitious.
		_swatchModel.Draw(Matrix.TR(bounds.center, Quat.FromAngles(0,0,bounds.center.x*70000+bounds.center.y*30000)), Color.HSV(hue, saturation, value));

		// If the users interacts with the volume the swatch model is in,
		// then we'll set the active color right here, and play some sfx!
		BtnState state = UI.VolumeAt(id, bounds, UIConfirm.Push);
		if (state.IsJustActive())
		{
			Sound.Click.Play(Hierarchy.ToWorld(bounds.center));
			SetColor(hue, saturation, value);
		}
		if (state.IsJustInactive())
			Sound.Unclick.Play(Hierarchy.ToWorld(bounds.center));
	}

	bool SwatchSize(string id, float size)
	{
		// Reserve a spot for this swatch!
		Bounds bounds = UI.LayoutReserve(Vec2.One * 4 * U.cm);
		bounds.dimensions.z = U.cm*4;

		// Draw a swatch model using the size it represents! We'll also
		// add some pseudo-random rotation to prevent it from looking too
		// repetitious.
		_swatchModel.Draw(Matrix.TRS(bounds.center, Quat.FromAngles(0, 0, bounds.center.x * 70000 + bounds.center.y * 30000), size*(1/.04f)), Color.HSV(_hue, _saturation, _value));

		// If the users interacts with the volume the swatch model is in,
		// then we'll notify the caller by returning true.
		BtnState state = UI.VolumeAt(id, bounds, UIConfirm.Push);
		if (state.IsJustActive())
		{
			Sound.Click.Play(Hierarchy.ToWorld(bounds.center));
			return true;
		}
		if (state.IsJustInactive())
			Sound.Unclick.Play(Hierarchy.ToWorld(bounds.center));
		return false;
	}

	void SetColor(float hue, float saturation, float value)
	{
		_hue        = hue;
		_saturation = saturation;
		_value      = value;
		_color      = Color.HSV(hue,saturation,value);

		// Update the ink bottle's material with the current color.
		_model.RootNode.Material[MatParamName.ColorTint] = _color;

		// And we'll also colorize the user's hand mesh too!
		Default.MaterialHand[MatParamName.ColorTint] = _color;
		// You could also set individual hands to a custom material using
		// Input.HandMaterial
	}
}