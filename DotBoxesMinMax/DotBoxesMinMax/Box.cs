using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBoxesMinMax
{
    // TODO ENUM FOR CAPTURE
    public class Box
    {
        // Coords for the 4 dots
        public Tuple<int, int> dotUpperLeft;
        public Tuple<int, int> dotUpperRight;
        public Tuple<int, int> dotBottomLeft;
        public Tuple<int, int> dotBottomRight;

        // The line is simply the coords of the two dots
        public Tuple<Tuple<int, int>, Tuple<int, int>> lineLeft;
        public Tuple<Tuple<int, int>, Tuple<int, int>> lineTop;
        public Tuple<Tuple<int, int>, Tuple<int, int>> lineRight;
        public Tuple<Tuple<int, int>, Tuple<int, int>> lineBottom;

        // Dictionary to check where the line is connected,
        // with tuple of tuples of ints as key (i.e. coords of two dots)
        // and bool as value (connected or not)
        public Dictionary<Tuple<Tuple<int, int>, Tuple<int, int>>, bool> lineConnectedDict = 
            new Dictionary<Tuple<Tuple<int, int>, Tuple<int, int>>, bool>();

        public int capturedBy = -1;

        public int numConnectedLines = 0;

        public Box(Tuple<int, int> coordUppderLeft) 
        { 
            dotUpperLeft = coordUppderLeft;
            dotUpperRight = Tuple.Create(coordUppderLeft.Item1, coordUppderLeft.Item2 + 1);
            dotBottomLeft = Tuple.Create(coordUppderLeft.Item1 + 1, coordUppderLeft.Item2);
            dotBottomRight = Tuple.Create(coordUppderLeft.Item1 + 1, coordUppderLeft.Item2 + 1);

            // Always declare line from top dot to bottom dot,
            // left dot to right dot
            lineLeft = Tuple.Create(dotUpperLeft, dotBottomLeft);
            lineTop = Tuple.Create(dotUpperLeft, dotUpperRight);
            lineRight = Tuple.Create(dotUpperRight, dotBottomRight);
            lineBottom = Tuple.Create(dotBottomLeft, dotBottomRight);

            lineConnectedDict.Add(lineLeft, false);
            lineConnectedDict.Add(lineTop, false);
            lineConnectedDict.Add(lineRight, false);
            lineConnectedDict.Add(lineBottom, false);
        }

        public Box(Box otherBox)
        {
            dotUpperLeft = otherBox.dotUpperLeft;
            dotUpperRight = otherBox.dotUpperRight;
            dotBottomLeft = otherBox.dotBottomLeft;
            dotBottomRight = otherBox.dotBottomRight;

            // Always declare line from top dot to bottom dot,
            // left dot to right dot
            lineLeft = otherBox.lineLeft;
            lineTop = otherBox.lineTop;
            lineRight = otherBox.lineRight;
            lineBottom = otherBox.lineBottom;

            lineConnectedDict.Add(lineLeft, otherBox.lineConnectedDict[lineLeft]);
            lineConnectedDict.Add(lineTop, otherBox.lineConnectedDict[lineTop]);
            lineConnectedDict.Add(lineRight, otherBox.lineConnectedDict[lineRight]);
            lineConnectedDict.Add(lineBottom, otherBox.lineConnectedDict[lineBottom]);

            capturedBy = otherBox.capturedBy;
            numConnectedLines = otherBox.numConnectedLines;
        }

        public void AddLinesToSet(
            HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> availableLines)
        {
            availableLines.Add(lineLeft);
            availableLines.Add(lineTop);
            availableLines.Add(lineRight);
            availableLines.Add(lineBottom);
        }

        public void ConnectDots(
            Tuple<Tuple<int, int>, Tuple<int, int>> lineToConnect, 
            int turnIndex)
        {
            lineConnectedDict[lineToConnect] = true;
            numConnectedLines += 1;
        }
    }
}
