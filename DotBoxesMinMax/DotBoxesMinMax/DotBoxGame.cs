using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotBoxesMinMax
{
    public class DotBoxGame
    {

        public Board board;
        public const int playersTurn = 0;
        public const int AIsTurn = 1;
        public int nextTurnIndex = 1;
        public int numOfLinesTotal;
        public Random randomizer = new Random();
        public DotBoxGame(int h, int w) 
        {
            board = new Board(h, w);
            numOfLinesTotal = h * (w - 1) + w * (h - 1);
            //Tuple<int, int> p1 = Tuple.Create(0, 1);
            //Tuple<int, int> p2 = Tuple.Create(1, 1);
            //var lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, playersTurn);

            //p1 = Tuple.Create(0, 0);
            //p2 = Tuple.Create(1, 0);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, AIsTurn);

            //p1 = Tuple.Create(1, 1);
            //p2 = Tuple.Create(1, 2);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, playersTurn);

            //p1 = Tuple.Create(1, 0);
            //p2 = Tuple.Create(2, 0);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, AIsTurn);

            //p1 = Tuple.Create(1, 1);
            //p2 = Tuple.Create(2, 1);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, playersTurn);

            //p1 = Tuple.Create(0, 1);
            //p2 = Tuple.Create(0, 2);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, AIsTurn);

            //p1 = Tuple.Create(0, 2);
            //p2 = Tuple.Create(1, 2);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, playersTurn);

            //p1 = Tuple.Create(2, 1);
            //p2 = Tuple.Create(2, 2);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, playersTurn);

            //p1 = Tuple.Create(1, 2);
            //p2 = Tuple.Create(2, 2);
            //lineToConnect = Tuple.Create(p1, p2);
            //board.MakeMove(lineToConnect, AIsTurn);
        }

        public void PlayersMove()
        {
            Console.WriteLine("YOUR TURN");
            string input = Console.ReadLine();
            var items = input.Split(',');
            var numbers = new List<int>();
            foreach (string item in items)
                if (int.TryParse(item, out var number))
                    numbers.Add(number);
            Tuple<int, int> p1 = Tuple.Create(numbers[0], numbers[1]);
            Tuple<int, int> p2 = Tuple.Create(numbers[2], numbers[3]);
            var lineToConnect = Tuple.Create(p1, p2);
            nextTurnIndex = board.MakeMove(lineToConnect, playersTurn);
        }

        public void AIsMove() 
        {
            Console.WriteLine("AI's TURN");

            Tuple<Tuple<int, int>, Tuple<int, int>> chosenLine = null;
            if (board.availableLines.Count >= (int)numOfLinesTotal / 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    var randomLine = board.availableLines.ElementAt(
                        randomizer.Next(board.availableLines.Count));

                    int firstDotRow = randomLine.Item1.Item1;
                    int firstDotCol = randomLine.Item1.Item2;
                    bool isHorizontal = board.IsHorizontalLine(randomLine);
                    int varToChange = (isHorizontal ? firstDotRow : firstDotCol);
                    bool bothBoxesPassed = true;

                    // If horizontal, check top and bottom boxes that share the same line
                    // if vertical, check left and right boxes that share the same line
                    for (int j = varToChange; j >= varToChange - 1; j--)
                    {

                        if (j < 0 ||
                            isHorizontal && j >= board.height - 1 ||
                            !isHorizontal && j >= board.width - 1)
                            continue;
                        bool checkPassed = false;
                        Box box = isHorizontal ? board.boxes[j][firstDotCol] : 
                            board.boxes[firstDotRow][j];

                        int numOfLinesConnectedForThisBox = box.CheckNumConnectedLines();

                        if (numOfLinesConnectedForThisBox < 2 ||
                            numOfLinesConnectedForThisBox == 3)
                            checkPassed = true;

                        bothBoxesPassed = bothBoxesPassed && checkPassed;
                    }
                    if (bothBoxesPassed)
                    {
                        chosenLine = randomLine;
                        break;
                    }
                }
            }

            if (chosenLine == null)
            {
                (float score, chosenLine) = MinMax.getScore(board, 10, -100000, 100000, AIsTurn);
            }
            nextTurnIndex = board.MakeMove(chosenLine, AIsTurn);
            Console.WriteLine(
                chosenLine.Item1.Item1.ToString() + ", " +
                chosenLine.Item1.Item2.ToString() + ", " +
                chosenLine.Item2.Item1.ToString() + ", " +
                chosenLine.Item2.Item2.ToString());
        }

        public void BeginPlay()
        {
            while(board.availableLines.Count > 0)
            {
                if(nextTurnIndex == playersTurn)
                {
                    PlayersMove();
                }
                else
                {
                    AIsMove();
                }
            }

            Console.WriteLine("GameOver");
            Console.WriteLine("Your Score:" + board.score[0]);
            Console.WriteLine("AI's Score:" + board.score[1]);
        }
    }
}
