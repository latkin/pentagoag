using System;
using System.Collections.Generic;

namespace PentagoAgEngine
{
    // Negamax based player which searches deeper and deeper until 
    // a given time limit elapses
    public class FixedTimePlayer : Player
    {
        private const int MINSCORE = -10000000;
        private const int MAXSCORE = 10000000;
        private const int NOMOVES = 10000001;
        private const int TIMEUP = 10000002;

        private DateTime startTime = DateTime.Now;
        private int thinkTimeSeconds = 10;

        public FixedTimePlayer(PieceColor c, int thinkTimeSeconds)
            : base(c)
        {
            if (thinkTimeSeconds > 0)
                this.thinkTimeSeconds = thinkTimeSeconds;
            else
                throw new ArgumentException("Think time must be greater than 0");
        }

        public override Board Play(Board b)
        {
            if (b.CurrentPlayer != this.Color)
                throw new InvalidOperationException();

            List<Board> boards = b.GenerateSafeBoards(true);

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
            Board bestBoardLastCompletedDepth = boards[0];
            int lastCompletedDepth = 1;
            int lastCompletedDepthScore = 0;
            
            startTime = DateTime.Now;

            for (int depth = 1; depth < 36; depth++)
            {
                localAlpha = MINSCORE;
                int i;
                for (i = 0; i < boards.Count; i++)
                {
                    current = -AlphaBeta(boards[i], depth, MINSCORE, -localAlpha);
                    if (current == -TIMEUP)
                        break;
                    if (current == -NOMOVES)
                        current = boards[i].Evaluation;

                    boards[i].Evaluation = current;

                    if (current > localAlpha)
                    {
                        localAlpha = current;
                        bestBoard = boards[i];
                        if (current == BoardEvaluator.EVAL_WHITE_WIN)
                        {
                            lastCompletedDepth = depth;
                            lastCompletedDepthScore = current;
                            bestBoardLastCompletedDepth = bestBoard;
                            break;
                        }
                    }
                }

                // bail from the loop if we didn't make it
                // all the way through the search at this depth
                if (i != boards.Count)
                    break;

                // otherwise save the best move
                lastCompletedDepthScore = localAlpha;
                lastCompletedDepth = depth;
                bestBoardLastCompletedDepth = bestBoard;

                // re-sort the boards after each completed depth
                //   to increase the number of cutoffs at the next depth
                BoardEvaluator.SortBoards(ref boards);
            }

            Console.WriteLine(String.Format("Depth: {0} Score: {1}", lastCompletedDepth, lastCompletedDepthScore));
            return bestBoardLastCompletedDepth;
        }

        // negamax style alpha beta pruning search
        private int AlphaBeta(Board b, int depth, int alpha, int beta)
        {
            DateTime currentTime = DateTime.Now;
            if ((currentTime - startTime).TotalSeconds > thinkTimeSeconds)
                return TIMEUP;

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

            BoardEvaluator.SortBoards(ref boards);

            int current;
            int best = MINSCORE;
            int localAlpha = alpha;

            for (int i = 0; i < boards.Count; i++)
            {
                current = -AlphaBeta(boards[i], depth - 1, -beta, -localAlpha);

                if (current == -NOMOVES)
                    current = boards[i].Evaluation;

                else if (current == -TIMEUP)
                    return TIMEUP;

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
