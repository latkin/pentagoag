using System;
using System.Collections.Generic;

namespace PentagoAgEngine
{
    public enum PieceColor
    {
        Empty = 0,  // no piece
        White = 1,  
        Black = 2,
        Both = 3    // used in draw situations
    }

    public enum Rotation
    {
        UpperLeftClockwise = 0,
        UpperLeftAntiClockwise,
        UpperRightClockwise,
        UpperRightAntiClockwise,
        LowerLeftClockwise,
        LowerLeftAntiClockwise,
        LowerRightClockwise,
        LowerRightAntiClockwise
    }


    /* 
     * Represents the board and all pieces.
     * Access positions by index, with lower
     * left being (0,0) and upper right being
     * (5,5).
     * 
     * Marble locations are stored in 2 bitboards,
     * which are left-justified.
     * For example:
     *  --- ---
     * |   |   |  White bit board: 100000000000000100000000000000000000...
     * |   |   |  Black bit board: 000100000000100000000000000000000000...
     * |   |   |
     *  --- ---
     * |B  |W  |
     * |   |   |
     * |W  |B  |
     *  --- ---
     */ 
    public class Board
    {
        internal UInt64 WhiteBitBoard;
        internal UInt64 BlackBitBoard;

        // evaluation set during sub-board generation
        // matches evaluation for CurrentPlayer
        public int Evaluation = 0;

        public PieceColor CurrentPlayer = PieceColor.White;     // this is only altered by ApplyMove, copy constructor, and Copy

        public Board()
        {
        }

        public Board(Board b)
        {
            this.WhiteBitBoard = b.WhiteBitBoard;
            this.BlackBitBoard = b.BlackBitBoard;
            this.CurrentPlayer = b.CurrentPlayer;
            this.Evaluation = b.Evaluation;
        }

        public void Copy(Board b)
        {
            this.WhiteBitBoard = b.WhiteBitBoard;
            this.BlackBitBoard = b.BlackBitBoard;
            this.CurrentPlayer = b.CurrentPlayer;
            this.Evaluation = b.Evaluation;
        }

        public void Clear()
        {
            WhiteBitBoard = BoardHelper.Zeros;
            BlackBitBoard = BoardHelper.Zeros;
            CurrentPlayer = PieceColor.White;
        }

        public PieceColor GetColorAt(int xCoord, int yCoord)
        {
            if ((WhiteBitBoard & BoardHelper.Bits[XY(xCoord, yCoord)]) != 0)
                return PieceColor.White;
            else if ((BlackBitBoard & BoardHelper.Bits[XY(xCoord, yCoord)]) != 0)
                return PieceColor.Black;

            return PieceColor.Empty;
        }

        // Put a piece at the location specified, or noop if a piece already there.
        // Empty color is placed no matter what.
        // Return true if piece was placed, false otherwise
        public bool PlacePieceAt(int xCoord, int yCoord, PieceColor color)
        {
            switch (color)
            {
                case PieceColor.Empty:
                    WhiteBitBoard &= BoardHelper.EmptyBits[XY(xCoord, yCoord)];
                    BlackBitBoard &= BoardHelper.EmptyBits[XY(xCoord, yCoord)];
                    return true;
                case PieceColor.White:
                    if (((BlackBitBoard | WhiteBitBoard) & BoardHelper.Bits[XY(xCoord, yCoord)]) != 0)
                        return false;
                    WhiteBitBoard |= BoardHelper.Bits[XY(xCoord, yCoord)];
                    return true;
                case PieceColor.Black:
                    if (((BlackBitBoard | WhiteBitBoard) & BoardHelper.Bits[XY(xCoord, yCoord)]) != 0)
                        return false;
                    BlackBitBoard |= BoardHelper.Bits[XY(xCoord, yCoord)];
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("Invalid piece color");
            }
        }

        // Applies the specified rotation to the board
        public void DoRotation(Rotation r)
        {
            int[] indices;
            switch (r)
            {
                case Rotation.UpperLeftClockwise:
                    indices = BoardHelper.UpperLeftClockwiseIndices;
                    break;
                case Rotation.UpperLeftAntiClockwise:
                    indices = BoardHelper.UpperLeftAntiClockwiseIndices;
                    break;
                case Rotation.UpperRightClockwise:
                    indices = BoardHelper.UpperRightClockwiseIndices;
                    break;
                case Rotation.UpperRightAntiClockwise:
                    indices = BoardHelper.UpperRightAntiClockwiseIndices;
                    break;
                case Rotation.LowerLeftClockwise:
                    indices = BoardHelper.LowerLeftClockwiseIndices;
                    break;
                case Rotation.LowerLeftAntiClockwise:
                    indices = BoardHelper.LowerLeftAntiClockwiseIndices;
                    break;
                case Rotation.LowerRightClockwise:
                    indices = BoardHelper.LowerRightClockwiseIndices;
                    break;
                case Rotation.LowerRightAntiClockwise:
                    indices = BoardHelper.LowerRightAntiClockwiseIndices;
                    break;
                default:
                    throw new ArgumentException("Invalid rotation type");
            }
        
            // do loops of 4
            // first loop rotate the corners, second loop rotates the sides
            for (int j = 0; j < 2; j++)
            {
                // temp storage
                bool whiteTemp = ((WhiteBitBoard & BoardHelper.Bits[indices[4*j]]) != 0);
                bool blackTemp = ((BlackBitBoard & BoardHelper.Bits[indices[4*j]]) != 0);
                for (int i = 4*j; i < 4*j + 3; i++)
                {
                    // if there is a white piece at the next location, set bit at current location to 1
                    if ((WhiteBitBoard & BoardHelper.Bits[indices[i + 1]]) != 0)
                        WhiteBitBoard |= BoardHelper.Bits[indices[i]];
                    // otherwise set bit at current location to 0
                    else
                        WhiteBitBoard &= BoardHelper.EmptyBits[indices[i]];

                    // same for black
                    if ((BlackBitBoard & BoardHelper.Bits[indices[i + 1]]) != 0)
                        BlackBitBoard |= BoardHelper.Bits[indices[i]];
                    else
                        BlackBitBoard &= BoardHelper.EmptyBits[indices[i]];
                }
                // last location
                if (whiteTemp)
                    WhiteBitBoard |= BoardHelper.Bits[indices[4 * j + 3]];
                else
                    WhiteBitBoard &= BoardHelper.EmptyBits[indices[4 * j + 3]];
                if (blackTemp)
                    BlackBitBoard |= BoardHelper.Bits[indices[4 * j + 3]];
                else
                    BlackBitBoard &= BoardHelper.EmptyBits[indices[4 * j + 3]];
            }
        }

        // Apply a full move to the board
        // This swaps CurrentPlayer
        public bool ApplyMove(int xCoord, int yCoord, PieceColor color, Rotation r)
        {
            bool placed = this.PlacePieceAt(xCoord, yCoord, color);
            if (placed)
            {
                this.DoRotation(r);
                this.CurrentPlayer = (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
            }
            return placed;
        }

        // gets the number of white pieces on the board
        // todo: faster to maintain incrementally?
        public int NumWhite()
        {
            int result = 0;
            for (int i = 0; i < 36; i++)
            {
                if ((WhiteBitBoard & BoardHelper.Bits[i]) != 0)
                    result++;
            }
            return result;
        }

        // gets the number of black pieces on the board
        // todo: faster to maintain incrementally?
        public int NumBlack()
        {
            int result = 0;
            for (int i = 0; i < 36; i++)
            {
                if ((BlackBitBoard & BoardHelper.Bits[i]) != 0)
                    result++;
            }
            return result;
        }

        public PieceColor GetWinner()
        {
            int winner = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((WhiteBitBoard & BoardHelper.WinningLines[i]) == BoardHelper.WinningLines[i])
                    winner |= 1;
                else if ((BlackBitBoard & BoardHelper.WinningLines[i]) == BoardHelper.WinningLines[i])
                    winner |= 2;
            }
            return (PieceColor)winner;
        }

        public bool IsDraw()
        {
            switch (this.GetWinner())
            {
                case PieceColor.White:
                case PieceColor.Black:
                    return false;
                case PieceColor.Both:
                    return true;
                case PieceColor.Empty:
                    if (this.NumBlack() + this.NumWhite() == 36)
                        return true;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("Invalid winner color");
            }
        }

        // Finds all unique boards possible after applying a move by the current player
        public List<Board> GenerateSubBoards()
        {
            Dictionary<Board, int> boards = new Dictionary<Board, int>(256);

            if (this.GetWinner() != PieceColor.Empty)
                return new List<Board>();

            Board b = new Board(this);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (this.GetColorAt(i, j) == PieceColor.Empty)
                    {
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.LowerLeftAntiClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.LowerLeftClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.LowerRightAntiClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.LowerRightClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.UpperLeftAntiClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.UpperLeftClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.UpperRightAntiClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                        b.Copy(this);
                        b.ApplyMoveFast(i, j, this.CurrentPlayer, Rotation.UpperRightClockwise);
                        if (!boards.ContainsKey(b))
                        {
                            b.Evaluation = BoardEvaluator.Evaluate(ref b);
                            boards.Add(new Board(b), 0);
                        }
                    }
                }
            }
            
            return new List<Board>(boards.Keys);
        }

        // Only generate sub boards where the opposing player cannot win on their next move
        public List<Board> GenerateSafeBoards()
        {
            return GenerateSafeBoards(false);
        }

        public List<Board> GenerateSafeBoards(bool includeCheckmates)
        {
            List<Board> boards = this.GenerateSubBoards();
            Board workingBoard = new Board();

            for (int k = 0; k < boards.Count; k++)
            {
                workingBoard.Copy(boards[k]);

                foreach (Board b in workingBoard.GenerateSubBoards())
                {
                    if (b.GetWinner() == workingBoard.CurrentPlayer)
                    {
                        boards.RemoveAt(k);
                        k--;
                        break;
                    }
                    if(includeCheckmates && 
                        BoardEvaluator.HasCheckmate(b, workingBoard.CurrentPlayer))
                    {
                        boards.RemoveAt(k);
                        k--;
                        break;
                    }
                }
            }
            return boards;
        }


        // Not called directly but needed for perf and accuracy
        //   as the Dictionary class will use it
        // Do not remove!!
        public override int GetHashCode()
        {
            return WhiteBitBoard.GetHashCode() ^ BlackBitBoard.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((Board)obj).BlackBitBoard == this.BlackBitBoard && ((Board)obj).WhiteBitBoard == this.WhiteBitBoard;
        }

        public override string ToString()
        {
            String result = " --- --- \r\n";
            for (int i = 5; i >= 0; i--)
            {
                if (i == 2)
                {
                    result += " ---+--- \r\n";
                }

                result += "|";
                for (int j = 0; j < 6; j++)
                {
                    if (j == 3)
                    {
                        result += "|";
                    }
                    switch (this.GetColorAt(j, i))
                    {
                        case PieceColor.Black:
                            result += "B";
                            break;
                        case PieceColor.White:
                            result += "W";
                            break;
                        case PieceColor.Empty:
                            result += " ";
                            break;
                    }
                }
                result += "|\r\n";
            }
            result += " --- --- ";
            return result;
        }

        // map the x,y coords to a single index
        private int XY(int x, int y)
        {
            return 6 * y + x;
        }

        // No check for existing piece, and only supports black/white
        private void PlacePieceAtFast(int xCoord, int yCoord, PieceColor color)
        {
            if (color == PieceColor.White)
                WhiteBitBoard |= BoardHelper.Bits[XY(xCoord, yCoord)];
            else
                BlackBitBoard |= BoardHelper.Bits[XY(xCoord, yCoord)];
        }


        // No error check
        private void ApplyMoveFast(int xCoord, int yCoord, PieceColor color, Rotation r)
        {
            PlacePieceAtFast(xCoord, yCoord, color);
            DoRotation(r);
            CurrentPlayer = (color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        }

    }
}
