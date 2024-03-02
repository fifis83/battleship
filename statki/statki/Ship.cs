using System.Collections.Generic;
using System.Linq;

namespace statki
{
    internal class Ship
    {
        public int length;
        public List<Coordinate> coordinates = new List<Coordinate>();
        public Ship(int length)
        {
            this.length = length;
        }

        public bool IsSunk(Board board)
        {
            return coordinates.All(c => board.GetCellState(c) == CellState.Hit);
        }

        public void Sink(Board board)
        {
            foreach (var coord in coordinates)
            {
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        int newRow = coord.row + dr;
                        int newCol = coord.col + dc;

                        // Check if within board boundaries
                        if (0 <= newRow && newRow < board.boardSize && 0 <= newCol && newCol < board.boardSize)
                        {
                            board.playfield[newRow, newCol] = CellState.Miss;
                        }

                    }
                }
            }

            coordinates.ForEach(c => { board.playfield[c.row, c.col] = CellState.Hit; });
            board.placedShips.Remove(this);
        }

    }
}
