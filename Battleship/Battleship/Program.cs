using System;

namespace statki
{

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Battleship";

            Player p1;
            Player p2;
        // choose game mode

        gameSelect:
            Console.WriteLine("Welcome to battleship!\n[1] player vs player [2] player vs pc");
            string gamechoice = Console.ReadLine();
            if (!(gamechoice.Trim() == "1" || gamechoice.Trim() == "2")) goto gameSelect;
            Console.WriteLine("Enter player 1 name: ");
        name1:
            // choose player 1 name
            p1 = new Player(Console.ReadLine());
            if (p1.name == "") { Console.CursorTop--; goto name1; }

            switch (gamechoice.Trim())
            {
                case "1":

                    // choose player 2 name
                    Console.WriteLine("Enter player 2 name: ");
                name2:
                    p2 = new Player(Console.ReadLine());
                    if (p2.name == "") { Console.CursorTop--; goto name2; }
                pvpGameStart:
                    p1.board.PlaceShips(p1);
                    p2.board.PlaceShips(p2);
                    p1.enemyBoard = p2.board;
                    p2.enemyBoard = p1.board;
                    while (true)
                    {
                        // player 1 turn
                        Console.Clear();
                        Console.WriteLine($"{p1.name}'s turn");
                        Console.WriteLine($"Press anything to continue");
                        Console.ReadKey();
                        p2.enemyBoard.PrintBoard(true);
                        Console.WriteLine($"Your board");
                        Console.WriteLine($"Press anything to continue");
                        Console.ReadKey();
                        if (p1.enemyBoard.Attack(p1))
                        {
                            p1.wins++;
                            Console.Clear();
                            Console.WriteLine($"Player {p1.name} won!");
                            break;
                        }

                        // player 2 turn
                        Console.Clear();
                        Console.WriteLine($"{p2.name}'s turn");
                        Console.WriteLine($"Press anything to continue");
                        Console.ReadKey();
                        p1.enemyBoard.PrintBoard(true);
                        Console.WriteLine($"Your board");
                        Console.WriteLine($"Press anything to continue");
                        Console.ReadKey();

                        if (p2.enemyBoard.Attack(p2))
                        {
                            p2.wins++;
                            Console.Clear();
                            Console.WriteLine($"Player {p2.name} won!");
                            break;

                        }
                    }
                // play again
                pvpPlayAgain:
                    Console.WriteLine("wins {0}: {1}  {2}: {3}", p1.name, p1.wins, p2.name, p2.wins);
                    Console.WriteLine("Do you want to play again? (Y/N): ");
                    string playerChoice = Console.ReadLine();
                    switch (playerChoice.Trim().ToUpper())
                    {
                        case "Y":
                            goto pvpGameStart;

                        case "N":
                            return;

                        default:
                            Console.WriteLine("Please input Y or N");
                            goto pvpPlayAgain;
                    }
                case "2":
                AIGameStart:
                    AI ai = new AI();
                    p1.board.PlaceShips(p1);
                    ai.enemyBoard = p1.board;
                    p1.enemyBoard = ai.board;
                    while (true)
                    {
                        // player's round
                        Console.Clear();
                        ai.enemyBoard.PrintBoard(true);
                        Console.WriteLine($"Your board");
                        Console.WriteLine($"Press anything to continue");
                        Console.ReadKey();
                        if (p1.enemyBoard.Attack(p1))
                        {
                            p1.wins++;
                            Console.Clear();
                            Console.WriteLine($"Player {p1.name} won!");
                            break;
                        }
                        // AI round
                        if (ai.enemyBoard.Attack(ai))
                        {
                            ai.wins++;
                            Console.Clear();
                            Console.WriteLine($"Player {ai.name} won! *)_*) (you might want to prepare for the revolution)");
                            break;
                        }
                    }
                    // play again
                    Console.WriteLine("wins {0}: {1}  {2}: {3}", p1.name, p1.wins, ai.name, ai.wins);
                    Console.WriteLine("Do you want to play again? (Y/N): ");
                AIPlayAgain:
                    string choice = Console.ReadLine();
                    switch (choice.Trim().ToUpper())
                    {
                        case "Y":

                            goto AIGameStart;

                        case "N":
                            return;

                        default:
                            Console.WriteLine("Please input Y or N");
                            goto AIPlayAgain;

                    }
                default:
                    break;
            }
        }
    }
}