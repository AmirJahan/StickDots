using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotBoxesMinMax
{
    public class MinMax
    {
        const int AITurnIndex = 1;

        public static (float, Tuple<Tuple<int, int>, Tuple<int, int>>) getScore(
            Board currentBoardState, 
            int currentDepth, 
            float alphaMax, 
            float betaMin,
            int currentTurnIndex)
        {
            float bestScore;
            Tuple<Tuple<int, int>, Tuple<int, int>> bestLine = null;

            if (currentDepth == 0 || 
                currentBoardState.availableLines.Count == 0)
            {
                return (
                    currentBoardState.score[AITurnIndex] -
                    currentBoardState.score[AITurnIndex - 1], 
                    bestLine);
            }

            if (currentTurnIndex == AITurnIndex)
                bestScore = -100000;

            else
                bestScore = 100000;

            //// Use copy constructor to create a new board for
            //// minmax score calculation
            //Board nextBoardState = new Board(currentBoardState);

            HashSet<Tuple<Tuple<int, int>, Tuple<int, int>>> availableLines =
                currentBoardState.availableLines;

            List<float> score = new List<float>();

            foreach (var line in availableLines)
            {
                Board nextBoardState = new Board(currentBoardState);
                int nextTurnIndex = nextBoardState.MakeMove(line, currentTurnIndex);

                (float nextMoveScore, var _) = getScore(
                    nextBoardState, 
                    currentDepth - 1, 
                    alphaMax, 
                    betaMin,
                    nextTurnIndex);

                score.Add(nextMoveScore);

                // If AI's turn, get the highest score,
                // if human's turn get the lowest score
                // i.e. lowest score for human => best for AI
                if (currentTurnIndex == AITurnIndex)
                {
                    //bestScore = Math.Max(bestScore, nextMoveScore);
                    if (nextMoveScore > bestScore)
                    {
                        bestScore = nextMoveScore;
                        bestLine = line;
                        //bestLine = (nextMoveLine == null)? line : nextMoveLine;
                    }
                    alphaMax = Math.Max(alphaMax, nextMoveScore);
                }
                else
                {
                    //bestScore = Math.Min(bestScore, nextMoveScore);
                    if (nextMoveScore < bestScore)
                    {
                        bestScore = nextMoveScore;
                        bestLine = line;
                        //bestLine = (nextMoveLine == null) ? line : nextMoveLine;
                    }
                    betaMin = Math.Min(betaMin, nextMoveScore);
                }

                // Alpha beta pruning
                if (betaMin <= alphaMax)
                    break;
            }

            return (bestScore, bestLine);
        }
    }
}
