using System;
using System.Collections.Generic;


namespace PentagoAgEngine
{
    // Helper class for evaluating and sorting boards
    public class BoardEvaluator
    {
        public const int EVAL_DRAW = 0;
        public const int EVAL_WHITE_WIN = 1000000;
        public const int EVAL_BLACK_WIN = -1000000;

        public static void SortBoards(ref List<Board> boards)
        {
            boards.Sort(CompareEvaluations);
        }

        private static int CompareEvaluations(Board a, Board b)
        {
            return -a.Evaluation.CompareTo(b.Evaluation);
        }

        // Gives high score to opposite color of .CurrentPlayer,
        // which is the color which presumably just played a move.
        // Negamax requires the evaluation function to keep track of
        // appropriately assigning +/- evaluations depending on who
        // is playing.
        public static int Evaluate(ref Board b)
        {
            int multiplier = (b.CurrentPlayer == PieceColor.White) ? -1 : 1;
   
            switch (b.GetWinner())
            {
                case PieceColor.Empty:
                    if (b.NumBlack() == 18)
                        return EVAL_DRAW;
                    break;
                case PieceColor.White:
                    return multiplier * EVAL_WHITE_WIN;
                case PieceColor.Black:
                    return multiplier * EVAL_BLACK_WIN;
                case PieceColor.Both:
                    return EVAL_DRAW;
            }

            return multiplier * EvaluateChains(ref b);
        }

        /* Evaluation heurisitc:
         *      Consider each potential winning line.  If there is a black piece
         *      in the line, then it cannot be won by white, so score that line as zero.
         *      Otherwise, score the line as the # of white pieces residing in the line.
         *      Keep track of the highest scoring chain, and how many were found with
         *      that score.  Do the same for black.
         *      
         *      Give most favor to longer chains, then secondary favor to the number of
         *      such chains present.  Create a positive white score this way, then
         *      subtract off the corresponding black score using the same logic. 
         *      
         *      Always gives positive score to white, negative to black.  Up to the
         *      caller to multiply by -1 if needed.
         */
        private static int EvaluateChains(ref Board b)
        {
            int longestWhiteChain = 0;
            int numLongestWhiteChain = 0;
            int longestBlackChain = 0;
            int numLongestBlackChain = 0;
            UInt64 whiteTemp, blackTemp;
            int temp;

            for (int i = 0; i < BoardHelper.WinningLines.Length; i++)
            {
                // how many white pieces are in this line
                whiteTemp = b.WhiteBitBoard & BoardHelper.WinningLines[i];
                // how many black pieces are in this line
                blackTemp = b.BlackBitBoard & BoardHelper.WinningLines[i];

                if (blackTemp == 0 && whiteTemp != 0)
                {
                    temp = BoardHelper.NumCommonOneBits(b.WhiteBitBoard, BoardHelper.WinningLines[i]);
                    if (temp >= longestWhiteChain)
                    {
                        if (temp == longestWhiteChain)
                            numLongestWhiteChain++;
                        else
                        {
                            longestWhiteChain = temp;
                            numLongestWhiteChain = 1;
                        }
                    }
                    continue;  // no need to evaluate black
                }
                if (whiteTemp == 0 && blackTemp != 0)
                {
                    temp = BoardHelper.NumCommonOneBits(b.BlackBitBoard, BoardHelper.WinningLines[i]);
                    if (temp >= longestBlackChain)
                    {
                        if (temp == longestBlackChain)
                            numLongestBlackChain++;
                        else
                        {
                            longestBlackChain = temp;
                            numLongestBlackChain = 1;
                        }
                    }
                }
            }

            return (100 * longestWhiteChain + numLongestWhiteChain) - (100 * longestBlackChain + numLongestBlackChain);
        }

        public static bool HasCheckmate(Board b, PieceColor pieceColor)
        {
            int whiteTemp = 0;
            int blackTemp = 0;
            UInt64 whiteLine = 0;
            UInt64 blackLine = 0;
            List<UInt64> lines = new List<UInt64>();

            for (int i = 0; i < BoardHelper.WinningLines.Length; i++)
            {
                whiteLine = b.WhiteBitBoard & BoardHelper.WinningLines[i];
                whiteTemp = BoardHelper.NumOneBits(whiteLine);
                blackLine = b.BlackBitBoard & BoardHelper.WinningLines[i];
                blackTemp = BoardHelper.NumOneBits(blackLine);

                if (pieceColor == PieceColor.White && blackTemp == 0 && whiteTemp == 4)
                {
                    foreach (UInt64 line in lines)
                    {
                        if (line == whiteLine)
                            return true;
                    }
                    lines.Add(whiteLine);
                }
                else if (pieceColor == PieceColor.Black && whiteTemp == 0 && blackTemp == 4)
                {
                    foreach (UInt64 line in lines)
                    {
                        if (line == blackLine)
                            return true;
                    }
                    lines.Add(blackLine);
                }
            }
            return false;
        }
    }
}
