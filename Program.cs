using System;
using StereoKit;
using StereoKit.Framework;

class Program
{
	static Painting    activePainting = new Painting();
	static PaletteMenu paletteMenu;
	static Pose        menuPose       = new Pose(0.4f, 0, -0.4f, Quat.LookDir(-1,0,1));
	static Sprite      appLogo;

	static void Main(string[] args)
	{
		// Initialize StereoKit! During initialization, we can prepare a few
		// settings, like the assetsFolder and appName. assetsFolder the
		// folder that StereoKit will look for assets in when provided a
		// relative folder name. Settings can also be told to make a
		// flatscreen app, or how to behave if the preferred initialization
		// mode fails.
		SKSettings settings = new SKSettings
		{
			appName      = "StereoKit Ink",
			assetsFolder = "Assets",
		};
		if (!SK.Initialize(settings))
			Environment.Exit(1);

		// This is a simple radial hand menu where we'll store some quick
		// actions! It's activated by a grip motion, and is great for fast,
		// gesture-like activation of menu items. It also can be used with
		// multiple HandRadialLayers to nest commands in sub-menus.
		//
		// Steppers are classes that implement the IStepper interface, and
		// once added to StereoKit's stepper list, will have their Step
		// method called each frame! This is a great way to add fire-and-
		// forget objects or systems that need to update each frame.
		SK.AddStepper(new HandMenuRadial(
			new HandRadialLayer("Root", -90,
				new HandMenuItem("Undo", null, ()=>activePainting?.Undo()),
				new HandMenuItem("Redo", null, ()=>activePainting?.Redo()))));

		// Initialize the palette menu, see PaletteMenu.cs! This class
		// manages the palette UI object for manipulating our brush stroke
		// size and color.
		paletteMenu = new PaletteMenu();

		// Load in the app logo as a sprite! We'll draw this at the top of
		// the application menu later in this file.
		appLogo = Sprite.FromFile("StereoKitInkLight.png");

		// Step the application each frame, until StereoKit is told to exit!
		// The callback code here is called every frame after input and
		// system events, but before the draw events!
		while (SK.Step(() =>
		{
			// Send input information to the painting, it will handle this
			// info to create brush strokes. This will also draw the painting
			// too!
			activePainting.Step(Handed.Right, paletteMenu.PaintColor, paletteMenu.PaintSize);

			// Step our palette UI!
			paletteMenu.Step();

			// Step our application's menu! This includes Save/Load Clear and
			// Quit commands.
			StepMenuWindow();
		}));

		// We're done! Clean up StereoKit and all its resources :)
		SK.Shutdown();
	}

	static void StepMenuWindow()
	{
		// Begin the application's menu window, we'll draw this without a
		// head bar (Body only) since we have a nice application image we can
		// add instead!
		UI.WindowBegin("Menu", ref menuPose, UIWin.Body);

		// Just draw the application logo across the top of the Menu window!
		// Vec2.Zero here tells StereoKit to auto-size both axes, so this
		// will automatically expand to the width of the window.
		UI.Image(appLogo, V.XY(UI.LayoutRemaining.x, 0));

		// Add undo and redo to the main menu, these are both available on
		// the radial menu, but these are easier to discover, and it never
		// hurts to have multiple options!
		if (UI.Button("Undo")) activePainting?.Undo();
		UI.SameLine();
		if (UI.Button("Redo")) activePainting?.Redo();

		// When the user presses the save button, lets show a save file
		// dialog! When a file name and folder have been selected, it'll make
		// a call to SavePainting with the file's path name with the .skp
		// extension.
		if (UI.Button("Save"))
			Platform.FilePicker(PickerMode.Save, SavePainting, null, ".skp");

		// And on that same line, we'll have a load button! This'll let the
		// user pick out any .skp files, and will call LoadPainting with the
		// selected file.
		UI.SameLine();
		if (UI.Button("Load"))
			Platform.FilePicker(PickerMode.Open, LoadPainting, null, ".skp");

		// Some visual separation
		UI.HSeparator();

		// Clear is easy! Just create a new Painting object!
		if (UI.Button("Clear"))
			activePainting = new Painting();

		// And if they want to quit? Just tell StereoKit! This will let
		// StereoKit finish the the frame properly, and then break out of the
		// Step loop above.
		UI.SameLine();
		if (UI.Button("Quit"))
			SK.Quit();

		// And end the window!
		UI.WindowEnd();
	}

	static void LoadPainting(string file)
		=> activePainting = Painting.FromFile(Platform.ReadFileText(file)??"");

	static void SavePainting(string file)
		=> Platform.WriteFile(file, activePainting.ToFileData());

}