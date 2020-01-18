using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StereoKitPaintTutorial
{
    class Painting
    {
        List<LinePoint>   _activeLine = new List<LinePoint>();
        List<LinePoint[]> _lineList   = new List<LinePoint[]>();

        Vec3 _prevFingertip;

        public bool IsDrawing { get; private set; }

        void BeginLine(Vec3 at, Color32 color, float thickness)
        {
            _activeLine.Clear();
            _activeLine.Add(new LinePoint(at, color, thickness));
            _activeLine.Add(new LinePoint(at, color, thickness));
            _prevFingertip = at;
            IsDrawing = true;
        }

        void UpdateLine(Vec3 at, Color32 color, float thickness)
        {
            if (!IsDrawing)
                return;

            Vec3      prev  = _activeLine[_activeLine.Count - 2].pt;
            float     dist  = Vec3.Distance(prev, at);
            float     speed = Vec3.Distance(at, _prevFingertip) * Time.Elapsedf;
            LinePoint here  = new LinePoint(at, color, Math.Max(1 - speed / 0.0003f, 0.1f) * thickness);
            _activeLine[_activeLine.Count - 1] = here;

            if (dist > 1 * Units.cm2m)
                _activeLine.Add(here);
        }

        void EndLine()
        {
            if (_activeLine.Count > 0)
                _lineList.Add(_activeLine.ToArray());

            _activeLine.Clear();
            IsDrawing = false;
        }

        public void UpdateInput(Handed handed, Color color, float thickness)
        {
            Hand hand = Input.Hand(handed);
            Vec3 tip  = hand[FingerId.Index, JointId.Tip].position;
            tip = Vec3.Lerp(_prevFingertip, tip, 0.3f);
            _prevFingertip = tip;

            if (hand.IsJustPinched && !UI.IsInteracting(handed))
                BeginLine(tip, color, thickness);
            if (hand.IsJustUnpinched)
                EndLine();

            UpdateLine(tip, color, thickness);
        }

        public void Draw()
        {
            Lines.Add(_activeLine.ToArray());
            for (int i = 0; i < _lineList.Count; i++)
                Lines.Add(_lineList[i]);
        }

        #region File Load and Save

        public static Painting FromFile(string fileData)
        {
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
