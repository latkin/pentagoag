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
using Microsoft.Phone.Controls;
using PentagoAgEngine;
using System.ComponentModel;
using System.Diagnostics;
using WindowsPhonePentago;
using Microsoft.Phone.Shell;
using System.Threading;

namespace WindowsPhonePentago
{
    public struct Move
    {
        public int xCoord;
        public int yCoord;
        public PieceColor pieceColor;
        public Rotation rotation;
    }

    public enum ColorTheme
    {
        Natural,
        Red
    }

    public enum GameStyle
    {
        VsAi,
        VsHuman
    }

    public partial class MainPage : PhoneApplicationPage
    {
        Board masterBoard = new Board();
        PieceColor playerColor = PieceColor.White;
        Player masterAI;
        private bool okToClickDots = false;
        private bool okToRotateQuadrant = false;
        private bool aiThinking = false;
        private ColorTheme currentTheme = ColorTheme.Natural;
        private GameStyle currentGameStyle = GameStyle.VsAi;
        public AppSettings settings = new AppSettings();

        BackgroundWorker aiBackGround = new BackgroundWorker();

        public MainPage()
        {
            InitializeComponent();

            UpperLeft.okToPlaceDot += OkToPlaceDotCallback;
            UpperRight.okToPlaceDot += OkToPlaceDotCallback;
            LowerLeft.okToPlaceDot += OkToPlaceDotCallback;
            LowerRight.okToPlaceDot += OkToPlaceDotCallback;

            UpperLeft.getColorForDot += GetColorCallback;
            UpperRight.getColorForDot += GetColorCallback;
            LowerLeft.getColorForDot += GetColorCallback;
            LowerRight.getColorForDot += GetColorCallback;

            UpperLeft.okToRotateQuadrant += OkToRotateQuadrantCallback;
            UpperRight.okToRotateQuadrant += OkToRotateQuadrantCallback;
            LowerLeft.okToRotateQuadrant += OkToRotateQuadrantCallback;
            LowerRight.okToRotateQuadrant += OkToRotateQuadrantCallback;

            UpperLeft.notifyDotPlaced += DotPlacedCallback;
            UpperRight.notifyDotPlaced += DotPlacedCallback;
            LowerLeft.notifyDotPlaced += DotPlacedCallback;
            LowerRight.notifyDotPlaced += DotPlacedCallback;

            UpperLeft.notifyQuadrantRotated += QuadrantRotatedCallback;
            UpperRight.notifyQuadrantRotated += QuadrantRotatedCallback;
            LowerLeft.notifyQuadrantRotated += QuadrantRotatedCallback;
            LowerRight.notifyQuadrantRotated += QuadrantRotatedCallback;           

            aiBackGround.DoWork += new DoWorkEventHandler(aiBackGround_DoWork);
            aiBackGround.RunWorkerCompleted += new RunWorkerCompletedEventHandler(aiBackGround_RunWorkerCompleted);

            ApplicationBarMenuItem settingsItem = new ApplicationBarMenuItem();
            settingsItem.Text = "Settings";
            settingsItem.IsEnabled = true;
            settingsItem.Click += SettingsButton_Click;

            ApplicationBar.MenuItems.Add(settingsItem);

            SetColorTheme(settings.ColorThemeSetting, true);
            this.currentGameStyle = settings.GameStyleSetting;
        }

        void aiBackGround_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DoAiMoveComplete((Board)e.Result);
        }

        private void DoAiMoveComplete(Board aiBoard)
        {
            FadeBottomText.Stop();

            Move aiMove = GetMoveFromBoards(masterBoard, aiBoard);
            Debug.Assert(false, String.Format("x: {0} y:{1} r:{2}", aiMove.xCoord, aiMove.yCoord, aiMove.rotation));

            if (aiMove.pieceColor != PieceColor.Empty)
            {
                AnimateMove(aiMove);

                masterBoard = new Board(aiBoard);
            }
            if (masterBoard.IsDraw())
            {
                okToClickDots = false;
                okToRotateQuadrant = false;
                BottomText.Text = "";
                PopupTextBoxText.Text = "draw!";

                Thread.Sleep(1000);
                PopupPanel.Visibility = System.Windows.Visibility.Visible;

            }
            else if (masterBoard.GetWinner() != PieceColor.Empty)
            {
                okToClickDots = false;
                okToRotateQuadrant = false;

                BottomText.Text = "";

                PopupTextBoxText.Text = String.Format("{0} wins!", masterBoard.GetWinner() == PieceColor.White ? "white" : "black");


                PopupPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                BottomText.Text = "your move";
                okToClickDots = true;
            }
            aiThinking = false;
        }

        private Move GetMoveFromBoards(Board currentBoard, Board aiBoard)
        {
            Move returnMove = new Move();
            if (currentBoard.Equals(aiBoard))
            {
                returnMove.pieceColor = PieceColor.Empty;
                return returnMove;
            }

            PieceColor moveColor = (aiBoard.CurrentPlayer == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            returnMove.pieceColor = moveColor;

            Board b = new Board(currentBoard);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    b.Copy(currentBoard);
                    if (b.GetColorAt(i, j) == PieceColor.Empty)
                    {
                        returnMove.xCoord = i;
                        returnMove.yCoord = j;

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.LowerLeftAntiClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.LowerLeftAntiClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.LowerLeftClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.LowerLeftClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.LowerRightAntiClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.LowerRightAntiClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.LowerRightClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.LowerRightClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.UpperLeftAntiClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.UpperLeftAntiClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.UpperLeftClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.UpperLeftClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.UpperRightAntiClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.UpperRightAntiClockwise;
                            return returnMove;
                        }

                        b.Copy(currentBoard);
                        b.PlacePieceAt(i, j, moveColor);
                        b.DoRotation(Rotation.UpperRightClockwise);
                        if (b.Equals(aiBoard))
                        {
                            returnMove.rotation = Rotation.UpperRightClockwise;
                            return returnMove;
                        }
                    }
                }
            }
            // shouldn't ever get here
            Debug.Assert(false, "Bogus move \r\n" + masterBoard.ToString());
            return returnMove;
        }

        void aiBackGround_DoWork(object sender, DoWorkEventArgs e)
        {
            Board b = (Board)e.Argument;
            if (b.NumWhite() == 0 && b.NumBlack() == 0 && masterAI.Color == PieceColor.White)
                e.Result = masterAI.PlayFirstMove();
            else
                e.Result = masterAI.Play((Board)e.Argument);
        }

        public bool OkToPlaceDotCallback()
        {
            return okToClickDots;
        }

        public bool OkToRotateQuadrantCallback()
        {
            return okToRotateQuadrant;
        }

        public PieceColor GetColorCallback()
        {
            return playerColor;
        }

        public void DotPlacedCallback(String quadrantName, int hCoord, int vCoord, PieceColor clr)
        {
            okToClickDots = false;
            okToRotateQuadrant = true;
            switch (quadrantName)
            {
                case "UpperLeft":
                    masterBoard.PlacePieceAt(hCoord, vCoord + 3, clr);
                    break;
                case "UpperRight":
                    masterBoard.PlacePieceAt(hCoord + 3, vCoord + 3, clr);
                    break;
                case "LowerLeft":
                    masterBoard.PlacePieceAt(hCoord, vCoord, clr);
                    break;
                case "LowerRight":
                    masterBoard.PlacePieceAt(hCoord + 3, vCoord, clr);
                    break;
            }
        }

        public void QuadrantRotatedCallback(String quadrantName, TwistDirection twist)
        {
            okToRotateQuadrant = false;
            switch (quadrantName)
            {
                case "UpperLeft":
                    if (twist == TwistDirection.Clockwise)
                        masterBoard.DoRotation(Rotation.UpperLeftClockwise);
                    else
                        masterBoard.DoRotation(Rotation.UpperLeftAntiClockwise);
                    break;
                case "UpperRight":
                    if (twist == TwistDirection.Clockwise)
                        masterBoard.DoRotation(Rotation.UpperRightClockwise);
                    else
                        masterBoard.DoRotation(Rotation.UpperRightAntiClockwise);
                    break;
                case "LowerLeft":
                    if (twist == TwistDirection.Clockwise)
                        masterBoard.DoRotation(Rotation.LowerLeftClockwise);
                    else
                        masterBoard.DoRotation(Rotation.LowerLeftAntiClockwise);
                    break;
                case "LowerRight":
                    if (twist == TwistDirection.Clockwise)
                        masterBoard.DoRotation(Rotation.LowerRightClockwise);
                    else
                        masterBoard.DoRotation(Rotation.LowerRightAntiClockwise);
                    break;
            }

            if (this.currentGameStyle == GameStyle.VsAi)
            {
                DoAiMove();
            }
            else if (this.currentGameStyle == GameStyle.VsHuman)
            {
                DoHumanMove();
            }
        }

        private void DoAiMove()
        {
            aiThinking = true;
            masterBoard.CurrentPlayer = masterAI.Color;
            aiBackGround.RunWorkerAsync(new Board(masterBoard));
            BottomText.Text = "thinking";
            FadeBottomText.Begin();
        }

        private void DoHumanMove()
        {
            if (masterBoard.IsDraw())
            {
                okToClickDots = false;
                okToRotateQuadrant = false;
                BottomText.Text = "";
                PopupTextBoxText.Text = "draw!";

                PopupPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else if (masterBoard.GetWinner() != PieceColor.Empty)
            {
                okToClickDots = false;
                okToRotateQuadrant = false;

                BottomText.Text = "";

                PopupTextBoxText.Text = String.Format("{0} wins!", masterBoard.GetWinner() == PieceColor.White ? "white" : "black");

                PopupPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                playerColor = playerColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
                masterBoard.CurrentPlayer = playerColor;
                okToClickDots = true;
                BottomText.Text = String.Format("{0}'s move", playerColor == PieceColor.White ? "white" : "black");
            }
        }

        private void AnimateMove(Move move)
        {
            if (move.xCoord <= 2 &&
                move.yCoord <= 2)
            {
                LowerLeft.ShowDotPlacement(move.xCoord % 3, move.yCoord % 3, move.pieceColor);
            }
            else if (move.xCoord <= 2 &&
                move.yCoord > 2)
            {
                UpperLeft.ShowDotPlacement(move.xCoord % 3, move.yCoord % 3, move.pieceColor);
            }
            else if (move.xCoord > 2 &&
                move.yCoord <= 2)
            {
                LowerRight.ShowDotPlacement(move.xCoord % 3, move.yCoord % 3, move.pieceColor);
            }
            else if (move.xCoord > 2 &&
                move.yCoord > 2)
            {
                UpperRight.ShowDotPlacement(move.xCoord % 3, move.yCoord % 3, move.pieceColor);
            }

            // System.Threading.Thread.Sleep(1000);
            UpperLeft.ClearValue(Canvas.ZIndexProperty);
            UpperRight.ClearValue(Canvas.ZIndexProperty);
            LowerLeft.ClearValue(Canvas.ZIndexProperty);
            LowerRight.ClearValue(Canvas.ZIndexProperty);

            switch (move.rotation)
            {
                case Rotation.UpperLeftClockwise:
                    UpperLeft.SetValue(Canvas.ZIndexProperty, 9);
                    UpperLeft.ShowTwist(TwistDirection.Clockwise);
                    break;
                case Rotation.UpperLeftAntiClockwise:
                    UpperLeft.SetValue(Canvas.ZIndexProperty, 9);
                    UpperLeft.ShowTwist(TwistDirection.AntiClockwise);
                    break;
                case Rotation.UpperRightClockwise:
                    UpperRight.SetValue(Canvas.ZIndexProperty, 9);
                    UpperRight.ShowTwist(TwistDirection.Clockwise);
                    break;
                case Rotation.UpperRightAntiClockwise:
                    UpperRight.SetValue(Canvas.ZIndexProperty, 9);
                    UpperRight.ShowTwist(TwistDirection.AntiClockwise);
                    break;
                case Rotation.LowerLeftClockwise:
                    LowerLeft.SetValue(Canvas.ZIndexProperty, 9);
                    LowerLeft.ShowTwist(TwistDirection.Clockwise);
                    break;
                case Rotation.LowerLeftAntiClockwise:
                    LowerLeft.SetValue(Canvas.ZIndexProperty, 9);
                    LowerLeft.ShowTwist(TwistDirection.AntiClockwise);
                    break;
                case Rotation.LowerRightClockwise:
                    LowerRight.SetValue(Canvas.ZIndexProperty, 9);
                    LowerRight.ShowTwist(TwistDirection.Clockwise);
                    break;
                case Rotation.LowerRightAntiClockwise:
                    LowerRight.SetValue(Canvas.ZIndexProperty, 9);
                    LowerRight.ShowTwist(TwistDirection.AntiClockwise);
                    break;
                default:
                    break;
            }

        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // ignore new game requests if the ai is running
            if (aiThinking)
                return;

            okToRotateQuadrant = false;
            okToClickDots = false;

            UpperLeft.Reset();
            UpperRight.Reset();
            LowerLeft.Reset();
            LowerRight.Reset();

            PopupPanel.Visibility = System.Windows.Visibility.Collapsed;

            masterBoard = new Board();

            this.currentGameStyle = settings.GameStyleSetting;

            if (this.currentGameStyle == GameStyle.VsAi && settings.PlayerColorSetting == PieceColor.Black)
            {
                playerColor = PieceColor.Black;

                if (0 == settings.AiStrengthSetting)
                {
                    Debug.Assert(false, "Easy");
                    masterAI = new FixedDepthPlayer(PieceColor.White, 0);
                }
                else if (1 == settings.AiStrengthSetting)
                {
                    Debug.Assert(false, "Med");
                    masterAI = new FixedDepthPlayer(PieceColor.White, 2);
                }
                else
                {
                    Debug.Assert(false, "Hard");
                    masterAI = new FixedTimePlayer(PieceColor.White, 5);
                }

                aiThinking = true;
                DoAiMove();
            }
            else if (this.currentGameStyle == GameStyle.VsAi && settings.PlayerColorSetting == PieceColor.White)
            {
                playerColor = PieceColor.White;
                if (0 == settings.AiStrengthSetting)
                       masterAI = new FixedDepthPlayer(PieceColor.Black, 0);
                else if (1 == settings.AiStrengthSetting)
                    masterAI = new FixedDepthPlayer(PieceColor.Black, 2);
                     else
                        masterAI = new FixedTimePlayer(PieceColor.Black, 5);
                okToClickDots = true;
                okToRotateQuadrant = false;
                BottomText.Text = "your move";
            }
            else if (this.currentGameStyle == GameStyle.VsHuman)
            {
                playerColor = PieceColor.White;

                okToClickDots = true;
                okToRotateQuadrant = false;
                BottomText.Text = "white's move";
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            PopupPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetColorTheme(ColorTheme theme, bool force)
        {
            if (force || this.currentTheme != theme)
            {
                this.currentTheme = theme;
                UpperLeft.SetColorTheme(theme);
                UpperRight.SetColorTheme(theme);
                LowerLeft.SetColorTheme(theme);
                LowerRight.SetColorTheme(theme);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetColorTheme(settings.ColorThemeSetting, false);
        }
    }
}

