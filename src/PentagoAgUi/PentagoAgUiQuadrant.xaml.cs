using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PentagoAgEngine;

namespace PentagoAgUi
{
    public enum QuadrantOrientation
    {
        Upright = 0,
        Clockwise,
        UpsideDown,
        AntiClockwise,
        Unknown
    }
    public enum TwistDirection
    {
        Clockwise = 0,
        AntiClockwise,
        None
    }
    public partial class PentagoAgUiQuadrant : UserControl
    {

        // callback types
        public delegate bool QuadrantRotateVerifier();
        public delegate void QuadrantRotatedNotifier(String quadrantName, TwistDirection twist);
        public delegate void DotPlacedNotifier(String quadrantName, int hCoord, int vCoord, PieceColor clr);

        // callbacks bubbled up striaght from the dots
        public PentagoAgUi.PentagoAgUiDot.DotClickVerifier okToPlaceDot;
        public PentagoAgUi.PentagoAgUiDot.DotClickColorGetter getColorForDot;

        // callbacks specific to the quadrant
        public DotPlacedNotifier notifyDotPlaced;
        public QuadrantRotateVerifier okToRotateQuadrant;
        public QuadrantRotatedNotifier notifyQuadrantRotated;

        private bool mouseDrag = false;
        private bool dragBlockOn = false;
        private bool sendTwistNotification = false;
        private Double origAngle = 0;
        private Double refAngle = 0;
        private TwistDirection latestTwistDirection;

        public PentagoAgUiQuadrant()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += PentagoQuadrant_MouseLeftButtonDown;
            this.MouseLeftButtonUp += new MouseButtonEventHandler(PentagoQuadrant_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(PentagoQuadrant_MouseMove);
            QuadrantRotation.Completed += new EventHandler(QuadrantRotation_Completed);

            NWDot.okToPlaceDot += OkToPlaceDotCallback;
            NDot.okToPlaceDot += OkToPlaceDotCallback;
            NEDot.okToPlaceDot += OkToPlaceDotCallback;
            WDot.okToPlaceDot += OkToPlaceDotCallback;
            CenterDot.okToPlaceDot += OkToPlaceDotCallback;
            EDot.okToPlaceDot += OkToPlaceDotCallback;
            SWDot.okToPlaceDot += OkToPlaceDotCallback;
            SDot.okToPlaceDot += OkToPlaceDotCallback;
            SEDot.okToPlaceDot += OkToPlaceDotCallback;

            NWDot.getColorForDot += GetColorCallback;
            NDot.getColorForDot += GetColorCallback;
            NEDot.getColorForDot += GetColorCallback;
            WDot.getColorForDot += GetColorCallback;
            CenterDot.getColorForDot += GetColorCallback;
            EDot.getColorForDot += GetColorCallback;
            SWDot.getColorForDot += GetColorCallback;
            SDot.getColorForDot += GetColorCallback;
            SEDot.getColorForDot += GetColorCallback;

            NWDot.notifyDotPlaced += DotPlacedCallback;
            NDot.notifyDotPlaced += DotPlacedCallback;
            NEDot.notifyDotPlaced += DotPlacedCallback;
            WDot.notifyDotPlaced += DotPlacedCallback;
            CenterDot.notifyDotPlaced += DotPlacedCallback;
            EDot.notifyDotPlaced += DotPlacedCallback;
            SWDot.notifyDotPlaced += DotPlacedCallback;
            SDot.notifyDotPlaced += DotPlacedCallback;
            SEDot.notifyDotPlaced += DotPlacedCallback;

        }

        private void QuadrantRotation_Completed(object sender, EventArgs args)
        {
            if (sendTwistNotification)
            {
                if (notifyQuadrantRotated != null)
                    notifyQuadrantRotated(this.Name, latestTwistDirection);
                sendTwistNotification = false;
            }
        }
        public bool OkToPlaceDotCallback()
        {
            if (okToPlaceDot != null)
                return okToPlaceDot();
            return false;
        }
        public PieceColor GetColorCallback()
        {
            if (getColorForDot != null)
                return getColorForDot();
            return PieceColor.Empty;
        }
        public void DotPlacedCallback(String dotName, PieceColor clr)
        {
            int hCoord = 0, vCoord = 0;
            GetDotIndicesFromName(dotName, ref hCoord, ref vCoord);
            if (notifyDotPlaced != null)
                notifyDotPlaced(this.Name, hCoord, vCoord, clr);
        }

        private QuadrantOrientation GetOrientation()
        {
            Double currentAngle = (double)MasterGrid.RenderTransform.GetValue(RotateTransform.AngleProperty);
            if (currentAngle % 360 == 0)
                return QuadrantOrientation.Upright;
            else if ((currentAngle - 90) % 360 == 0)
                return QuadrantOrientation.Clockwise;
            else if ((currentAngle - 180) % 360 == 0)
                return QuadrantOrientation.UpsideDown;
            else if ((currentAngle - 270) % 360 == 0)
                return QuadrantOrientation.AntiClockwise;
            else
                return QuadrantOrientation.Unknown;
        }

        private void GetDotIndicesFromName(string dotName, ref int hCoord, ref int vCoord)
        {
            if (dotName == "CenterDot")
            {
                hCoord = 1;
                vCoord = 1;
                return;
            }
            switch (GetOrientation())
            {
                case QuadrantOrientation.Upright:
                    switch (dotName)
                    {
                        case "NWDot": hCoord = 0; vCoord = 2; break;
                        case "NDot": hCoord = 1; vCoord = 2; break;
                        case "NEDot": hCoord = 2; vCoord = 2; break;
                        case "WDot": hCoord = 0; vCoord = 1; break;
                        case "EDot": hCoord = 2; vCoord = 1; break;
                        case "SWDot": hCoord = 0; vCoord = 0; break;
                        case "SDot": hCoord = 1; vCoord = 0; break;
                        case "SEDot": hCoord = 2; vCoord = 0; break;
                    }
                    break;

                case QuadrantOrientation.Clockwise:
                    switch (dotName)
                    {
                        case "NWDot": hCoord = 2; vCoord = 2; break;
                        case "NDot": hCoord = 2; vCoord = 1; break;
                        case "NEDot": hCoord = 2; vCoord = 0; break;
                        case "WDot": hCoord = 1; vCoord = 2; break;
                        case "EDot": hCoord = 1; vCoord = 0; break;
                        case "SWDot": hCoord = 0; vCoord = 2; break;
                        case "SDot": hCoord = 0; vCoord = 1; break;
                        case "SEDot": hCoord = 0; vCoord = 0; break;
                    }
                    break;
                case QuadrantOrientation.UpsideDown:
                    switch (dotName)
                    {
                        case "NWDot": hCoord = 2; vCoord = 0; break;
                        case "NDot": hCoord = 1; vCoord = 0; break;
                        case "NEDot": hCoord = 0; vCoord = 0; break;
                        case "WDot": hCoord = 2; vCoord = 1; break;
                        case "EDot": hCoord = 0; vCoord = 1; break;
                        case "SWDot": hCoord = 2; vCoord = 2; break;
                        case "SDot": hCoord = 1; vCoord = 2; break;
                        case "SEDot": hCoord = 0; vCoord = 2; break;
                    }
                    break;
                case QuadrantOrientation.AntiClockwise:
                    switch (dotName)
                    {
                        case "NWDot": hCoord = 0; vCoord = 0; break;
                        case "NDot": hCoord = 0; vCoord = 1; break;
                        case "NEDot": hCoord = 0; vCoord = 2; break;
                        case "WDot": hCoord = 1; vCoord = 0; break;
                        case "EDot": hCoord = 1; vCoord = 2; break;
                        case "SWDot": hCoord = 2; vCoord = 0; break;
                        case "SDot": hCoord = 2; vCoord = 1; break;
                        case "SEDot": hCoord = 2; vCoord = 2; break;
                    }
                    break;
                case QuadrantOrientation.Unknown:
                    break;
            }
        }
        private string GetDotNameFromIndices(int hCoord, int vCoord)
        {
            if (hCoord == 1 && vCoord == 1)
                return "CenterDot";

            int testH = 0, testV = 0;

            GetDotIndicesFromName("NWDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "NWDot";
            GetDotIndicesFromName("NDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "NDot";
            GetDotIndicesFromName("NEDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "NEDot";
            GetDotIndicesFromName("WDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "WDot";
            GetDotIndicesFromName("EDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "EDot";
            GetDotIndicesFromName("SWDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "SWDot";
            GetDotIndicesFromName("SDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "SDot";
            GetDotIndicesFromName("SEDot", ref testH, ref testV);
            if (testH == hCoord && testV == vCoord) return "SEDot";

            return null;
        }

        void PentagoQuadrant_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDrag)
            {
                Double currentAngle = (double)MasterGrid.RenderTransform.GetValue(RotateTransform.AngleProperty);
                Point pos = e.GetPosition(MasterCanvas);
                Point center = new Point(MasterCanvas.ActualWidth / 2, MasterCanvas.ActualHeight / 2);
                Double newAngle = Math.Atan2(
                    (center.Y - pos.Y), (pos.X - center.X)
                    );
                newAngle *= (360 / (2 * Math.PI));
                Double delta = newAngle - refAngle;

                if (Math.Abs(delta) >= 90 && Math.Abs(delta) <= 270)
                {
                    if (dragBlockOn)
                        return;
                    if (Math.Abs(delta) <= 180)
                        delta = (delta > 0) ? 90 : -90;
                    else
                        delta = (delta > 0) ? 270 : -270;

                    dragBlockOn = true;
                }
                else
                {
                    dragBlockOn = false;
                }
                Double newToAngle = origAngle - (delta);
                Duration dur = new Duration(TimeSpan.FromSeconds(0.01));

                ((DoubleAnimation)QuadrantRotation.Children[0]).From = currentAngle;
                ((DoubleAnimation)QuadrantRotation.Children[0]).To = newToAngle;
                ((DoubleAnimation)QuadrantRotation.Children[0]).Duration = dur;

                QuadrantRotation.ClearValue(Storyboard.BeginTimeProperty);
                QuadrantRotation.Begin();
            }
        }
        void PentagoQuadrant_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (okToRotateQuadrant != null && okToRotateQuadrant())
            {
                sendTwistNotification = false;
                mouseDrag = true;

                this.SetValue(Canvas.ZIndexProperty, 9);

                origAngle = (double)MasterGrid.RenderTransform.GetValue(RotateTransform.AngleProperty);
                Point pos = e.GetPosition(MasterCanvas);

                Point center = new Point(MasterCanvas.ActualWidth / 2, MasterCanvas.ActualHeight / 2);
                refAngle = Math.Atan2(
                    (center.Y - pos.Y), (pos.X - center.X)
                    );
                refAngle *= (360 / (2 * Math.PI));
                this.CaptureMouse();
            }
        }
        void PentagoQuadrant_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDrag)
            {
                mouseDrag = false;

                Double currentAngle = (double)MasterGrid.RenderTransform.GetValue(RotateTransform.AngleProperty);
                Duration dur = new Duration(TimeSpan.FromSeconds(0.1));
                Double to = currentAngle;

                if ((currentAngle % 90) != 0)
                {
                    Double mod360 = currentAngle % 360;
                    Double mod45 = currentAngle % 45;
                    if (mod360 < 0) { mod360 += 360; }
                    if (mod45 < 0) { mod45 += 45; }

                    to = currentAngle - mod45;

                    if ((mod360 <= 90 && mod360 > 45) ||
                        (mod360 <= 360 && mod360 > 315) ||
                        (mod360 <= 270 && mod360 > 225) ||
                        (mod360 <= 180 && mod360 > 135))
                    {
                        to += 45;
                    }
                }
                ((DoubleAnimation)QuadrantRotation.Children[0]).From = currentAngle;
                ((DoubleAnimation)QuadrantRotation.Children[0]).To = to;
                ((DoubleAnimation)QuadrantRotation.Children[0]).Duration = dur;
                QuadrantRotation.ClearValue(Storyboard.BeginTimeProperty);
                TwistDirection direction = GetTwistDirectionFromAngles(origAngle, to);
                if (direction != TwistDirection.None)
                {
                    sendTwistNotification = true;
                    latestTwistDirection = direction;
                }
                this.ReleaseMouseCapture();
                QuadrantRotation.Begin();
                this.ClearValue(Canvas.ZIndexProperty);
            }
        }

        private TwistDirection GetTwistDirectionFromAngles(double from, double to)
        {
            if (Mod360(from + 90) == Mod360(to))
                return TwistDirection.Clockwise;
            if (Mod360(from - 90) == Mod360(to))
                return TwistDirection.AntiClockwise;
            if (Mod360(from) == Mod360(to))
                return TwistDirection.None;

            //  Debug.Assert(false, String.Format("Invalid twist. From: {0} To: {1}", from, to));
            return TwistDirection.None;
        }
        private double Mod360(double input)
        {
            double temp = input % 360;
            if (temp < 0)
            {
                return temp + 360;
            }
            return temp;
        }
        internal void ShowDotPlacement(int hCoord, int vCoord, PieceColor clr)
        {
            switch (GetDotNameFromIndices(hCoord, vCoord))
            {
                case "NWDot":
                    NWDot.ShowDotPlacement(clr);
                    break;
                case "NDot":
                    NDot.ShowDotPlacement(clr);
                    break;
                case "NEDot":
                    NEDot.ShowDotPlacement(clr);
                    break;
                case "WDot":
                    WDot.ShowDotPlacement(clr);
                    break;
                case "CenterDot":
                    CenterDot.ShowDotPlacement(clr);
                    break;
                case "EDot":
                    EDot.ShowDotPlacement(clr);
                    break;
                case "SWDot":
                    SWDot.ShowDotPlacement(clr);
                    break;
                case "SDot":
                    SDot.ShowDotPlacement(clr);
                    break;
                case "SEDot":
                    SEDot.ShowDotPlacement(clr);
                    break;
                case null:
                    //  Debug.Assert(false, "Invalid dot name");
                    break;
            }
        }
        internal void ShowTwist(TwistDirection twistDirection)
        {
            Double currentAngle = (double)MasterGrid.RenderTransform.GetValue(RotateTransform.AngleProperty);
            ((DoubleAnimation)QuadrantRotation.Children[0]).From = currentAngle;
            ((DoubleAnimation)QuadrantRotation.Children[0]).To =
                (twistDirection == TwistDirection.Clockwise) ? currentAngle + 90 : currentAngle - 90;
            ((DoubleAnimation)QuadrantRotation.Children[0]).Duration = new Duration(TimeSpan.FromSeconds(1));
            QuadrantRotation.BeginTime = TimeSpan.FromSeconds(1);
            QuadrantRotation.Begin();
        }

        public void Reset()
        {
            ((DoubleAnimation)QuadrantRotation.Children[0]).From = 0;
            ((DoubleAnimation)QuadrantRotation.Children[0]).To = 0;
            ((DoubleAnimation)QuadrantRotation.Children[0]).Duration = new Duration(TimeSpan.FromSeconds(0.1));
            QuadrantRotation.ClearValue(Storyboard.BeginTimeProperty);
            QuadrantRotation.Begin();

            NWDot.Reset();
            NDot.Reset();
            NEDot.Reset();
            WDot.Reset();
            CenterDot.Reset();
            EDot.Reset();
            SWDot.Reset();
            SDot.Reset();
            SEDot.Reset();
        }
    }
}

