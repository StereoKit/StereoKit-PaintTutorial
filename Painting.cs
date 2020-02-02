using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StereoKitPaintTutorial
{
    class Painting
    {
        Pose               _pose       = new Pose(new Vec3(0, 0, -0.3f), Quat.Identity);
        List<LinePoint>    _activeLine = new List<LinePoint>();
        List<LinePoint[]>  _lineList   = new List<LinePoint[]>();
        Stack<LinePoint[]> _undoStack  = new Stack<LinePoint[]>();

        Vec3 _prevFingertip;
        bool _isDrawing;

        public void Step(Handed handed, Color color, float thickness)
        {
            // We'll enclose the whole painting with an affordance, so we can
            // move the painting around while we work with it.
            UI.AffordanceBegin("PaintingRoot", ref _pose, new Bounds(Vec3.One * 5 * Units.cm2m), true);

            UpdateInput(handed, color, thickness);
            Draw();

            UI.AffordanceEnd();
        }

        public void Undo()
        {
            // No undo if there's nothing in the painting
            if (_lineList.Count == 0)
                return;

            // Push the last line into the undo stack, and remove from the painting!
            _undoStack.Push(_lineList.Last());
            _lineList.RemoveAt(_lineList.Count-1);
        }

        public void Redo()
        {
            // Nothing to redo? No redo!
            if (_undoStack.Count == 0)
                return;

            // Pop the most recent Undo off the stack, and add it to the painting.
            _lineList.Add(_undoStack.Pop());
        }

        void UpdateInput(Handed handed, Color color, float thickness)
        {
            // Get the hand's fingertip, convert it to local space, and smooth
            // it out to reduce any jagged noise.
            Hand hand = Input.Hand(handed);
            Vec3 tip  = hand[FingerId.Index, JointId.Tip].position;
            tip = Hierarchy.ToLocal(tip);
            tip = Vec3.Lerp(_prevFingertip, tip, 0.3f);
            
            // If the user just made a pinching motion, and is not interacting
            // with the UI, we'll begin a paint stroke!
            if (hand.IsJustPinched && !UI.IsInteracting(handed))
            { 
                BeginLine(tip, color, thickness);
                _isDrawing = true;
            }
            // And when they cease the pinching motion, we'll end whatever line
            // we started.
            if (_isDrawing && hand.IsJustUnpinched)
            {
                EndLine();
                _isDrawing = false;
            }
            // If we're drawing a paint stroke, then lets update it with the current
            // steps information!
            if (_isDrawing)
                UpdateLine(tip, color, thickness);

            _prevFingertip = tip;
        }

        void Draw()
        {
            // Draw the unfinished line the user may be drawing
            Lines.Add(_activeLine.ToArray());

            // Then draw all the other lines that are part of the painting!
            for (int i = 0; i < _lineList.Count; i++)
                Lines.Add(_lineList[i]);
        }

        void BeginLine(Vec3 at, Color32 color, float thickness)
        {
            // Start with two points! The first one begins at the point provided,
            // and the second one will always be updated to the current fingertip
            // location, to prevent 'popping'
            _activeLine.Add(new LinePoint(at, color, thickness));
            _activeLine.Add(new LinePoint(at, color, thickness));
            _prevFingertip = at;
        }

        void UpdateLine(Vec3 at, Color32 color, float thickness)
        {
            // Calculate the current distance from the last point, as well as the
            // speed at which the hand is traveling.
            Vec3  prevLinePoint = _activeLine[_activeLine.Count - 2].pt;
            float dist  = Vec3.Distance(prevLinePoint, at);
            float speed = Vec3.Distance(at, _prevFingertip) / Time.Elapsedf;

            // Create a point at the current location, using speed as the thickness
            // of the line! The last point in the line should always be at the current
            // fingertip location to prevent 'popping' when adding a new point.
            LinePoint here  = new LinePoint(at, color, Math.Max(1 - speed * 0.5f, 0.1f) * thickness);
            _activeLine[_activeLine.Count - 1] = here;

            // If we're more than a centimeter away from our last point, we'll add
            // a new point! This is simple, but effective enough. A higher quality
            // implementation might use an error/change function that also factors
            // into account the change in angle.
            if (dist > 1 * Units.cm2m)
                _activeLine.Add(here);
        }

        void EndLine()
        {
            // Add the active line to the painting, and clear it out for the next one!
            _lineList.Add(_activeLine.ToArray());
            _activeLine.Clear();
        }

        #region File Load and Save

        public static Painting FromFile(string fileData)
        {
            // Here we're using Linq to parse a file! Linq is a Functional way of 
            // writing code that can be pretty great once you get used to it. Linq
            // should probably not be used in performance critical sections, but it's
            // acceptable enough for discreet events.
            //
            // In this file, each line is a paint stroke, and each point on that
            // stroke is separated by a comma. Each item within a point is separated
            // by spaces, which is taken care of in LinePointFromString.
            //
            // Example of a two stroke painting, two points in the first stroke (white), 
            // and three points in the second stroke (red):
            // 0 0 0 255 255 255 0.01, 0.1 0 0 255 255 255 0.01
            // 0 0.1 0 255 0 0 0.02, 0.1 0.1 0 255 0 0 0.02, 0.2 0 0 255 0 0 0.02
            Painting result = new Painting();
            result._lineList = fileData
                .Split('\n')
                .Select( textLine => textLine
                    .Split(',')
                    .Select(textPoint => LinePointFromString(textPoint))
                    .ToArray())
                .ToList();
            return result;
        }

        public string ToFileData()
        {
            // To convert this painting to a file is pretty simple! We have LinePointToString
            // which we can use for each point, and then we just have to join all the data
            // together. Each paint stroke goes on its own line using '\n', and each point
            // on that stroke separated with a comma.
            //
            // Example of a two stroke painting, two points in the first stroke (white), 
            // and three points in the second stroke (red):
            // 0 0 0 255 255 255 0.01, 0.1 0 0 255 255 255 0.01
            // 0 0.1 0 255 0 0 0.02, 0.1 0.1 0 255 0 0 0.02, 0.2 0 0 255 0 0 0.02
            return string.Join('\n', _lineList
                .Select(line => string.Join(',', line
                    .Select(point => LinePointToString(point)))));
        }

        static string LinePointToString(LinePoint point)
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}",
                point.pt   .x, point.pt   .y, point.pt   .z,
                point.color.r, point.color.g, point.color.b,
                point.thickness);
        }

        static LinePoint LinePointFromString(string point)
        {
            string[]  values = point.Split(' ');
            LinePoint result = new LinePoint();
            result.pt   .x = float.Parse(values[0]);
            result.pt   .y = float.Parse(values[1]);
            result.pt   .z = float.Parse(values[2]);
            result.color.r = byte .Parse(values[3]);
            result.color.g = byte .Parse(values[4]);
            result.color.b = byte .Parse(values[5]);
            result.color.a = 255;
            result.thickness = float.Parse(values[6]);
            return result;
        }

        #endregion
    }
}
