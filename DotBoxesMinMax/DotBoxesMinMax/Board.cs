using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DotBoxesMinMax
{
    public class Board
    {
        // Num of dots on verticle axis
        public int height;
        // Num of dots on horizontal axis
        public int width;

        public float[] score = [0, 0];

        public Box[][] boxes;

        // HashSet for unique elements
        // (i.e. no repeating when adding the same element)
        public HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> availableLines = 
            new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>();
        public HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> connectedLines = 
            new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>();

        public Queue<Tuple<Tuple<int, int>, Tuple<int, int>>> lastLineForBoxesWithThreeConnections = 
            new Queue<Tuple<Tuple<int, int>, Tuple<int, int>>>();

        public Board(int h, int w)
        {
            height = h;
            width = w;
            boxes = new Box[height - 1][];
            InitializeBoxes();
        }

        // Copy constructor
        // Need to copy board to make future moves in minmax algorithm
        public Board(Board originalBoardState)
        {
            height = originalBoardState.height;
            width = originalBoardState.width;
            score = [originalBoardState.score[0], originalBoardState.score[1]];
            boxes = new Box[height - 1][];
            boxes = InitializeBoxes(originalBoardState.boxes);

            // Shallow copy of the hashsets
            availableLines = 
                new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>
                (originalBoardState.availableLines);
            connectedLines =
                new HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>>
                (originalBoardState.connectedLines);

            // Shallow copy
            lastLineForBoxesWithThreeConnections.Clear();
            foreach (var line in originalBoardState.lastLineForBoxesWithThreeConnections)
            {
                lastLineForBoxesWithThreeConnections.Enqueue(line);
            }
        }

        public Box[][] InitializeBoxes()
        {
            for (int i = 0; i < height - 1; i++)
            {
                boxes[i] = new Box[width - 1];
                for (int j = 0; j < width - 1; j++)
                {
                    boxes[i][j] = new Box(Tuple.Create(i, j));
                    boxes[i][j].AddLinesToSet(availableLines);
                }
            }
            return boxes;
        }

        public Box[][] InitializeBoxes(Box[][] originalBoxes)
        {
            for (int i = 0; i < height - 1; i++)
            {
                boxes[i] = new Box[width - 1];
                for (int j = 0; j < width - 1; j++)
                {
                    // Use copy constructor of box
                    boxes[i][j] = new Box(originalBoxes[i][j]);
                }
            }
            return boxes;
        }

        public void PrintBoard()
        {

        }

        public int MakeMove(
            // NOTE: Always declare line from top dot to bottom dot,
            // left dot to right dot
            Tuple<Tuple<int, int>, Tuple<int, int>> lineToConnect,
            int turnIndex)
        {
            if (connectedLines.Contains(lineToConnect))
                return -1;

            connectedLines.Add(lineToConnect);
            availableLines.Remove(lineToConnect);

            // Connect the line and get the number of connected line for each box
            // Note: 1 move will affect 2 boxes
            int[] numConnectedLines = CheckBothBoxConnections(true, turnIndex, lineToConnect);

            bool captured = CheckIfEitherBoxCaptured(numConnectedLines);

            if (captured)
                score[turnIndex] += 1;

            // Flip the index if the person making the move didn't capture a box
            if (!captured)
                turnIndex = 1 - turnIndex;

            return turnIndex;
        }

        public bool CheckIfEitherBoxCaptured(int[] numConnectedLines)
        {
            foreach (int connections in numConnectedLines)
            {
                if (connections == 4) return true;
            }
            return false;
        }

        public int[] CheckBothBoxConnections(bool toConnect, int turnIndex,
            Tuple<Tuple<int, int>, Tuple<int, int>> lineToConnect)
        {
            int[] numConnectionsEachBox = new int[2];
            int firstDotRow = lineToConnect.Item1.Item1;
            int firstDotCol = lineToConnect.Item1.Item2;

            bool isHorizontal = IsHorizontalLine(lineToConnect);
            int varToChange = (isHorizontal ? firstDotRow : firstDotCol);
            int index = 0;
            for (int i = varToChange; i >= varToChange - 1; i--)
            {

                if (i < 0 ||
                    isHorizontal && i >= height - 1 ||
                    !isHorizontal && i >= width - 1)
                    continue;

                Box box = isHorizontal ? boxes[i][firstDotCol] : boxes[firstDotRow][i];

                if (toConnect)
                    box.ConnectDots(lineToConnect, turnIndex);

                numConnectionsEachBox[index] = box.numConnectedLines;

                if (box.numConnectedLines == 3)
                    AddLastLineToList(box);

                index++;
            }

            return numConnectionsEachBox;
        }

        public void AddLastLineToList(Box box)
        {
            foreach (KeyValuePair<Tuple<Tuple<int, int>, Tuple<int, int>>, bool> entry in 
                box.lineConnectedDict)
            {
                // Add to list if the line is not connected
                if (!entry.Value)
                    lastLineForBoxesWithThreeConnections.Enqueue(entry.Key);
            }
        }

        public bool IsHorizontalLine(Tuple<Tuple<int, int>, Tuple<int, int>> line)
        {
            // If the row index of first dot equals to the 
            // row index of the second dot, then horizontal line
            if (line.Item1.Item1 == line.Item2.Item1)
                return true;
            return false;
        }
    }
}
