using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace statki
{
    public enum CellState
    {
        Empty,
        Ship,
        Miss,
        Hit,
        Cursor,
    }

    internal class Board
    {
        public CellState[,] playfield;
        public List<Ship> placedShips = new List<Ship>();
        public int
            boardSize;
        public Board(int boardSize = 10)
        {
            this.boardSize = boardSize;
            playfield = new CellState[boardSize, boardSize];
            ResetBoard();
        }
        private void ResetBoard()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    playfield[i, j] = CellState.Empty;
                }
            }


            foreach (Ship ship in placedShips)
            {
                foreach (var coord in ship.coordinates)
                {
                    playfield[coord.row, coord.col] = CellState.Ship;
                }
            }
        }
        public void PrintBoard(bool displayShips)
        {

            Console.Clear();
            Console.Write("\x1b[3J");
            Console.WriteLine("  | A | B | C | D | E | F | G | H | I | J | ");
            Console.WriteLine("--+---+---+---+---+---+---+---+---+---+---+--");


            for (int i = 0; i < 10; i++)
            {
                Console.Write(i == 9 ? $"{i + 1}" : $" {i + 1}");
                for (int j = 0; j < 10; j++)
                {
                    Console.Write($"|");
                    switch (playfield[i, j])
                    {
                        case CellState.Empty:
                            Console.Write($"   ");
                            break;
                        case CellState.Ship:
                            Console.Write(displayShips ? " S " : "   ");
                            break;
                        case CellState.Hit:
                            Console.Write($" X ");
                            break;
                        case CellState.Miss:
                            Console.Write($" O ");
                            break;
                        case CellState.Cursor:
                            Console.Write($" * ");
                            break;
                        default:
                            break;
                    }
                }
                Console.Write($"|");
                Console.WriteLine();
                Console.WriteLine("--+---+---+---+---+---+---+---+---+---+---+--");
            }
            Console.WriteLine();
        }

        public void PlaceShips(Player player)
        {
            int[] shipsLeft = { 4, 3, 2, 1 };
            PrintBoard(true);
            Console.WriteLine($"Player {player.name} get ready to place your ship:");
            while (placedShips.Count < 10)
            {
                Console.WriteLine("Enter ship length(1-4): ");
                int shipLength;
                // check for correct length
                while (true)
                {
                    if (!int.TryParse(Console.ReadLine(), out shipLength))
                    {
                        Console.WriteLine("Woopsy! You entered a wrong value, try again!");
                        continue;
                    }
                    if (shipLength > 4 || shipLength < 0)
                    {
                        Console.WriteLine("Woopsy! You entered a wrong value, try again!");
                        continue;
                    }
                    if (shipsLeft[shipLength - 1] == 0)
                    {
                        Console.WriteLine("Woopsy! You have no ships of that length left, try again!");
                        continue;
                    }
                    break;
                }

                Ship newShip = new Ship(shipLength);

                bool placementValid = false;
                // Starting coordinates for the ship 
                Coordinate cursorCoord = new Coordinate(4, 4);
                string direction = "horizontal";


                while (!placementValid)
                {
                    ResetBoard();
                    for (int j = 0; j < 2; j++)
                    {
                        for (int i = 0; i < shipLength; i++)
                        {
                            int newRow = cursorCoord.row + (direction == "vertical" ? i : 0);
                            int newCol = cursorCoord.col + (direction == "horizontal" ? i : 0);
                            playfield[newRow, newCol] = j == 0 ? CellState.Ship : CellState.Empty; // Mark temporary ship placement
                        }
                        if (j == 0) PrintBoard(true);

                    }
                    ResetBoard();
                    Console.WriteLine("Use arrow keys to move, Enter to confirm, Esc to cancel, R to rotate.");

                    ConsoleKey key = Console.ReadKey(true).Key;
                    // input handling
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            cursorCoord.row = Math.Max(cursorCoord.row - 1, 0);
                            break;
                        case ConsoleKey.DownArrow:
                            if ((cursorCoord.row + shipLength < boardSize && direction == "vertical") || (cursorCoord.row < boardSize - 1 && direction == "horizontal")) cursorCoord.row += 1;
                            break;
                        case ConsoleKey.LeftArrow:
                            cursorCoord.col = Math.Max(cursorCoord.col - 1, 0);
                            break;
                        case ConsoleKey.RightArrow:
                            if ((cursorCoord.col + shipLength < boardSize && direction == "horizontal") || (cursorCoord.col < boardSize - 1 && direction == "vertical")) cursorCoord.col += 1;
                            break;
                        case ConsoleKey.R:
                            // Rotate Ship
                            if (direction == "horizontal")
                            {
                                if (cursorCoord.row + shipLength > boardSize)
                                {
                                    cursorCoord.row -= cursorCoord.row + shipLength - boardSize;
                                }
                                direction = "vertical";
                            }
                            else
                            {
                                if (cursorCoord.col + shipLength > boardSize)
                                {
                                    cursorCoord.col -= cursorCoord.col + shipLength - boardSize;
                                }
                                direction = "horizontal";

                            }
                            break;
                        case ConsoleKey.Enter:
                            // check if ship placement is correct i.e. colliding with other ships
                            for (int g = 0; g < newShip.length; g++)
                            {
                                int newRow = cursorCoord.row + (direction == "vertical" ? g : 0);
                                int newCol = cursorCoord.col + (direction == "horizontal" ? g : 0);
                                newShip.coordinates.Add(new Coordinate(newRow, newCol));
                            }
                            if (placementValid = TryPlaceShip(newShip)) break;
                            newShip.coordinates.Clear();

                            break;
                        case ConsoleKey.Escape:
                            Console.WriteLine("Ship placement cancelled.");
                            return;
                        default:
                            break;
                    }
                }
                shipsLeft[shipLength - 1] -= 1;
                placedShips.Add(newShip);
            }
            ResetBoard();

        }

        public CellState GetCellState(Coordinate coord)
        {
            return playfield[coord.row, coord.col];
        }

        private bool TryPlaceShip(Ship ship)
        {


            foreach (var coord in ship.coordinates)
            {


                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        int newRow = coord.row + dr;
                        int newCol = coord.col + dc;

                        // Check if within board boundaries
                        if (0 <= newRow && newRow < boardSize && 0 <= newCol && newCol < boardSize)
                        {
                            if (GetShipAtCoordinate(new Coordinate(newRow, newCol)) != null) return false;
                        }

                    }
                }
            }
            return true;
        }

        public bool Attack(Player player)
        {

            Coordinate cursor = new Coordinate(4, 4);
            bool stopAttack = false;
            while (!stopAttack)
            {
                CellState tempCell = playfield[cursor.row, cursor.col];
                playfield[cursor.row, cursor.col] = CellState.Cursor; // Mark temporary ship placement
                PrintBoard(false);
                playfield[cursor.row, cursor.col] = tempCell; // Mark temporary ship placement
                Console.WriteLine($"{player.name} use arrow keys to move cursor, Enter to shoot");


                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        cursor.row = Math.Max(cursor.row - 1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                        cursor.row = Math.Min(cursor.row + 1, boardSize - 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        cursor.col = Math.Max(cursor.col - 1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        cursor.col = Math.Min(cursor.col + 1, boardSize - 1);
                        break;
                    case ConsoleKey.Enter:

                        switch (GetCellState(cursor))
                        {
                            case CellState.Hit:
                                break;
                            case CellState.Ship:
                                playfield[cursor.row, cursor.col] = CellState.Hit;
                                Ship ship = GetShipAtCoordinate(cursor);
                                if (ship.IsSunk(this)) GetShipAtCoordinate(cursor).Sink(this);
                                if (placedShips.Count == 0) return true;
                                break;
                            case CellState.Empty:
                                playfield[cursor.row, cursor.col] = CellState.Miss;
                                stopAttack = true;
                                break;
                            case CellState.Miss:
                                break;

                            default: break;

                        }
                        break;

                    default: break;
                }
            }

            return false;
        }

        public bool Attack(AI ai)
        {
            Random random = new Random();
            Coordinate lastHit = null;
            Coordinate roll;
            int tryCounter = 0;
            while (true)
            {
                if (tryCounter == 0) lastHit = null;
                if (lastHit != null)
                {
                    int row = random.Next(-1, 2);
                    int col = random.Next(-1, 2);
                    row = row < 0 ? Math.Max(lastHit.row + row, 0) : Math.Min(lastHit.row + row, boardSize - 1);
                    col = col < 0 ? Math.Max(lastHit.col + col, 0) : Math.Min(lastHit.col + col, boardSize - 1);
                    roll = new Coordinate(row, col);
                }
                else
                {
                    roll = new Coordinate(random.Next(boardSize), random.Next(boardSize));
                }

                switch (GetCellState(roll))
                {
                    case CellState.Hit:
                        break;
                    case CellState.Ship:
                        playfield[roll.row, roll.col] = CellState.Hit;
                        lastHit = roll;
                        tryCounter = 3;
                        Ship ship = GetShipAtCoordinate(roll);
                        if (ship.IsSunk(this))
                        {
                            GetShipAtCoordinate(roll).Sink(this);
                            lastHit = null;
                        }
                        if (placedShips.Count == 0) return true;
                        break;
                    case CellState.Empty:
                        tryCounter--;
                        playfield[roll.row, roll.col] = CellState.Miss;
                        return false;
                    case CellState.Miss:
                        break;

                    default: break;

                }
            }


        }

        public Ship GetShipAtCoordinate(Coordinate coord)
        {
            return placedShips.FirstOrDefault(ship => ship.coordinates.Any(c => c.row == coord.row && c.col == coord.col));
        }

        public void PlaceShipsRandomly()
        {
            placedShips.Clear();
            int[] shipsLeft = { 4, 3, 2, 1 };

            foreach (int shipAmount in shipsLeft)
            {
                for (int i = 0; i < shipAmount; i++)
                {
                    int n = 0;
                    while (true)
                    {
                        if (n > 1000)
                        {
                            Console.WriteLine("Your ships have been lost in shipping, L+ratio.\nRestart the application ");
                            while (true) Console.ReadKey(true);
                        }
                        Random random = new Random();
                        int length = 5 - shipAmount;
                        Ship ship = new Ship(length);
                        string direction = random.Next(2) == 0 ? "horizontal" : "vertical";
                        Coordinate coord = new Coordinate(random.Next(boardSize), random.Next(boardSize));
                        if (direction == "vertical")
                        {
                            if (coord.row + ship.length > boardSize)
                            {
                                coord.row -= coord.row + ship.length - boardSize;
                            }
                        }
                        else
                        {
                            if (coord.col + ship.length > boardSize)
                            {
                                coord.col -= coord.col + ship.length - boardSize;
                            }
                        }
                        for (int g = 0; g < length; g++)
                        {
                            int newRow = coord.row + (direction == "vertical" ? g : 0);
                            int newCol = coord.col + (direction == "horizontal" ? g : 0);
                            ship.coordinates.Add(new Coordinate(newRow, newCol));
                        }
                        if (!TryPlaceShip(ship)) continue;
                        placedShips.Add(ship);
                        break;
                    }
                }

            }
            ResetBoard();
        }
    }

}

