

namespace statki
{
    internal class AI : Player
    {
        public Board board = new Board();
        public Board enemyBoard = new Board();
        public int wins=0;
        public AI(string name = "AdelAId") : base(name){
            board.PlaceShipsRandomly();
        }

    }
}
