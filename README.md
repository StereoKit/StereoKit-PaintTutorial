![](Assets/StereoKitInkLight.png)

This is a demo application, and introductory tutorial for StereoKit aimed at introducing people to the basics of how to draw and interact with Mixed Reality content! It just so happens that "inking" is a relatively straightforward thing to code, and is pretty fun at the same time.

You'll find out how to create a core application loop, radial hand menus, easy window menus with automatic layout, object-based menus with more explicit layouts, and file pickers. You'll also see how easy it is to draw models, lines, and interact directly with your fingers!

![](Docs/StereoKitInk.jpg)

While I could have gone wild on adding features, this project is intended as a readable, easy to understand learning resource! It's an example of how to do things, and a good starting point for those that might be interested in creating a similar feature for themselves. It's not meant to be an exhaustive or fully featured product.

## Pre-requisites

This project uses:
- [StereoKit](https://stereokit.net/Pages/Guides/Getting-Started.html)

This project uses [StereoKit](https://stereokit.net/) to render and drive this as a Mixed Reality application, which allows us to run on HoloLens 2 and VR headsets! That's all that's in here besides the tutorial code :)

## Project Layout

Since this is a pretty simple tutorial, there's not a lot of files here! But there are some things to make note of. This solution uses a 2 project setup: one is .Net Core, and one is UWP. Different projects support different features and targets, and I often switch between them based on what I'm working on.

- .Net Core Project
  - WMR VR Desktop
  - Flatscreen Desktop
  - Leap Motion articulated hands
  - No compile time
- UWP Project
  - HoloLens 2 + articulated hands
  - WMR VR Desktop
  - Flatscreen Desktop
  - Controller simulated hands
  - Some compile time

The project consists of only 3 code files, and a few art assets! The code aims to be very readable, and is also rich with comments to explain less intuitive items.

- [Program.cs](Program.cs)
  - This contains the application logic, the hand menu, and the application menu! It's a great place to start, since the application menu ties everything together.
- [PaletteMenu.cs](PaletteMenu.cs)
  - This is a menu that controls painting options. It's mostly composed of a built-in UI elements, but also illustrates how to use StereoKit's layout and interaction tools to create your own.
- [Painting.cs](Painting.cs)
  - This class encapsulates the idea of the finger painting itself! It manages hand input, and converts it into 3D lines. It also is responsible for saving and loading painting files. You can find use of StereoKit's Hierarchy system here as well.

## Questions or problems?

If you've got questions about how this works, or how to get it running, let me know over in the Issues tab! Alternatively, you can find me online over on [Twitter - @koujaku](https://twitter.com/koujaku), feel free to send me a note there too!
