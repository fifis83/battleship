namespace statki
{
    internal class Player
    {
        public string name;
        public Board board = new Board();
        public Board enemyBoard = new Board();
        public int wins = 0;

        public Player(string name)
        {
            this.name = name;
        }

    }
}