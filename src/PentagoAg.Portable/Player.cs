using System;

namespace PentagoAgEngine
{
    // base class, not much to see here
    public abstract class Player
    {
        public PieceColor Color;

        public Player(PieceColor c)
        {
            this.Color = c;
        }

        public abstract Board Play(Board b);

        public Board PlayFirstMove()
        {
            if (Color != PieceColor.White)
                throw new InvalidOperationException("Can't play first move as black");

            Board b = new Board();
            Random rnd = new Random();
            int x = rnd.Next(1, 5);
            int y = rnd.Next(1, 5);
            b.PlacePieceAt(x, y, PieceColor.White);
            b.CurrentPlayer = PieceColor.Black;
            return b;
        }
    }
}
