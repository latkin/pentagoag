using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PentagoAgEngine
{
    // Negamax based player which always searches to a given depth
    public class FixedDepthPlayer : Player
    {
        private const int MINSCORE = -10000000;
        private const int MAXSCORE = 10000000;
        private const int NOMOVES = 10000001;
        private int maxDepth = 3;
        int positionsEvaluated = 0;

        public FixedDepthPlayer(PieceColor c, int depth)
            : base(c)
        {
            if (depth >= 0)
                maxDepth = depth;
            else
                throw new ArgumentException("Depth must be at least 0");
        }


        public override Board Play(Board b)
        {
            if (b.CurrentPlayer != this.Color)
                throw new InvalidOperationException();

            Debug.Assert(false, b.ToString());

            List<Board> boards;
            if (maxDepth == 2)
                boards = b.GenerateSafeBoards(true);
            else
                boards = b.GenerateSafeBoards();

            Console.Write("Safe moves: " + boards.Count.ToString());

            // if opponent wins on their next move no matter what
            if (boards.Count == 0)
            {
                // if we have a legal move, do it even though it's a loser
                boards = b.GenerateSubBoards();
                if(boards.Count == 0)
                    return b;
                return boards[0];
            }

            BoardEvaluator.SortBoards(ref boards);

            int current;
            int localAlpha = MINSCORE;
            Board bestBoard = boards[0];

            DateTime startTime = DateTime.Now;
            positionsEvaluated = 0;
            
            for (int i = 0; i < boards.Count; i++)
            {
              //  Console.Write("+");

                current = -AlphaBeta(boards[i], maxDepth, MINSCORE, -localAlpha);
                if (current == -NOMOVES)
                    current = boards[i].Evaluation;

                if (current > localAlpha)
                {
                    localAlpha = current;
                    bestBoard = boards[i];
                    if (current == BoardEvaluator.EVAL_WHITE_WIN)
                        break;
                }
            }

            DateTime endTime = DateTime.Now;
            TimeSpan moveTime = endTime - startTime;

            //Console.WriteLine(" Score: " + localAlpha.ToString());
            //Console.WriteLine("Time: {0}", moveTime.TotalSeconds);
            //Console.WriteLine("Moves evaluated: {0}", positionsEvaluated);
            //Console.WriteLine("Moves/sec: {0}", positionsEvaluated / moveTime.TotalSeconds);

            Debug.Assert(false, bestBoard.ToString());
            return bestBoard;
        }

        // Negamax style alpha beta pruning search
        private int AlphaBeta(Board b, int depth, int alpha, int beta)
        {
            if (depth == 0)
                return -b.Evaluation;

            switch (b.GetWinner())
            {
                case PieceColor.White:
                    return (b.CurrentPlayer == PieceColor.White) ? BoardEvaluator.EVAL_WHITE_WIN : -BoardEvaluator.EVAL_WHITE_WIN;
                case PieceColor.Black:
                    return (b.CurrentPlayer == PieceColor.White) ? BoardEvaluator.EVAL_BLACK_WIN : -BoardEvaluator.EVAL_BLACK_WIN;
                case PieceColor.Both:
                    return BoardEvaluator.EVAL_DRAW;
                case PieceColor.Empty:
                    break;
            }

            List<Board> boards = b.GenerateSubBoards();
            if (boards.Count == 0)
                return NOMOVES;

            positionsEvaluated += boards.Count;

            BoardEvaluator.SortBoards(ref boards);

            int current;
            int best = MINSCORE;
            int localAlpha = alpha;

            for (int i = 0; i < boards.Count; i++)
            {
                current = -AlphaBeta(boards[i], depth - 1, -beta, -localAlpha);
                if (current == -NOMOVES)
                    current = boards[i].Evaluation;

                best = Math.Max(current, best);

                if (best >= beta)
                    return beta;

                if (best > localAlpha)
                    localAlpha = best;
            }
            return best;
        }
    }
}
