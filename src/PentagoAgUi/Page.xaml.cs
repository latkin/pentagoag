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
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace PentagoAgUi
{
    public struct Move
    {
        public int xCoord;
        public int yCoord;
        public PieceColor pieceColor;
        public Rotation rotation;
    }
    public partial class Page : UserControl
    {
        Board masterBoard = new Board();
        PieceColor playerColor = PieceColor.White;
        Player masterAI;
        private bool okToClickDots = false;
        private bool okToRotateQuadrant = false;
        private bool aiThinking = false;
        BackgroundWorker aiBackGround = new BackgroundWorker();

        public Page()
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
                BottomText.Text = "Draw!";
                okToClickDots = false;
                okToRotateQuadrant = false;
            }
            else if (masterBoard.GetWinner() != PieceColor.Empty)
            {
                if (masterBoard.GetWinner() == playerColor)
                {
                    BottomText.Text = "You win!";
                }
                else
                {
                    BottomText.Text = "Computer wins!";
                }
                okToClickDots = false;
                okToRotateQuadrant = false;
            }
            else
            {
                BottomText.Text = "Your move";
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

            PieceColor moveColor = (aiBoard.CurrentPlayer == PieceColor.White) ? PieceColor.Black: PieceColor.White;

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
            this.Cursor = Cursors.Hand;
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
            this.Cursor = Cursors.Arrow;
            switch (quadrantName)
            {
                case "UpperLeft":
                    if(twist == TwistDirection.Clockwise)
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

            DoAiMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ignore new game requests if the ai is running
            if (aiThinking)
                return;

            HowTo.Visibility = Visibility.Collapsed;
            HowToText.Visibility = Visibility.Collapsed;

            okToRotateQuadrant = false;
            okToClickDots = false;

            UpperLeft.Reset();
            UpperRight.Reset();
            LowerLeft.Reset();
            LowerRight.Reset();

            masterBoard = new Board();

            if ((bool)playerBlackRadioButton.IsChecked)
            {
                playerColor = PieceColor.Black;

                if ((bool)aiEasyRadioButton.IsChecked)
                {
                    Debug.Assert(false, "Easy");
                    masterAI = new FixedDepthPlayer(PieceColor.White, 0);
                }
                else if ((bool)aiMediumRadioButton.IsChecked)
                {
                    Debug.Assert(false,"Med");
                    masterAI = new FixedDepthPlayer(PieceColor.White, 2);
                }
                else
                {
                    Debug.Assert(false, "Hard");
                    masterAI = new FixedTimePlayer(PieceColor.White, 30);
                }

                aiThinking = true;
                DoAiMove();
            }
            else
            {
                playerColor = PieceColor.White;
                if ((bool)aiEasyRadioButton.IsChecked)
                    masterAI = new FixedDepthPlayer(PieceColor.Black, 0);
                else if ((bool)aiMediumRadioButton.IsChecked)
                    masterAI = new FixedDepthPlayer(PieceColor.Black, 2);
                else
                    masterAI = new FixedTimePlayer(PieceColor.Black, 30);
                okToClickDots = true;
                okToRotateQuadrant = false;
                BottomText.Text = "Your move";
            }
        }

        private void DoAiMove()
        {
            aiThinking = true;
            masterBoard.CurrentPlayer = masterAI.Color;
            aiBackGround.RunWorkerAsync(new Board(masterBoard));
            BottomText.Text = "Thinking...";
            FadeBottomText.Begin();
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
    }
}
