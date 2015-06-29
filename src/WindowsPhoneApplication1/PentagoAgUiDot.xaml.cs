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

namespace WindowsPhonePentago
{
    public partial class PentagoDot : UserControl
    {
        public delegate bool DotClickVerifier();
        public delegate PieceColor DotClickColorGetter();
        public delegate void DotPlacedNotifier(String dotName, PieceColor clr);

        public DotClickVerifier okToPlaceDot;
        public DotClickColorGetter getColorForDot;
        public DotPlacedNotifier notifyDotPlaced;

        public string id;

        private PieceColor currentColor = PieceColor.Empty;
        private ColorTheme theme = ColorTheme.Natural;

        public PentagoDot()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(PentagoDot_MouseLeftButtonDown);
        }

        public void ShowDotPlacement(PieceColor clr)
        {
            SetColor(clr);
        }

        public void SetColor(PieceColor clr)
        {
            this.currentColor = clr;

            if (clr == PieceColor.White)
            {
                this.Elly.Fill = (Brush)this.Resources["WhiteDotFill"];
            }
            else if (clr == PieceColor.Black)
            {
                this.Elly.Fill = (Brush)this.Resources["BlackDotFill"];
            }
            else if (clr == PieceColor.Empty)
            {
                if (theme == ColorTheme.Natural)
                {
                    this.Elly.Fill = (Brush)this.Resources["EmptyDotFillNatural"];
                }
                else if (theme == ColorTheme.Red)
                {
                    this.Elly.Fill = (Brush)this.Resources["EmptyDotFillRed"];
                }
            }
        }

        void PentagoDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (okToPlaceDot == null ||
                getColorForDot == null)
            {
                return;
            }
            if (this.currentColor == PieceColor.Empty &&
                okToPlaceDot())
            {
                PieceColor col = getColorForDot();
                SetColor(col);
                if (notifyDotPlaced != null)
                    notifyDotPlaced(this.Name, col);
            }
        }

        internal void Reset()
        {
            SetColor(PieceColor.Empty);
        }

        internal void SetColorTheme(ColorTheme theme)
        {
            this.theme = theme;

            if (this.currentColor == PieceColor.Empty)
            {
                SetColor(PieceColor.Empty);
            }
        }
    }
}
