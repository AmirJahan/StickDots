using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIRandom : Player
{
    private HashSet<Tuple<Vector2, Vector2>> _availableLines;
    private Tuple<Vector2, Vector2> _choosenLine;
    private int _randomIndex;

    public override void BeginTurn()
    {
        base.BeginTurn();

        _availableLines = GamePlayManager.Instance.board.AvailableLines;
        _randomIndex = UnityEngine.Random.Range(0, _availableLines.Count - 1);
        _choosenLine = _availableLines.ElementAt(_randomIndex);

        LineController.Instance.MakeLine(_choosenLine.Item1, _choosenLine.Item2);
        GamePlayManager.Instance.PlayersMove(_choosenLine.Item1, _choosenLine.Item2);
        Debug.Log($"AI{GamePlayManager.Instance.currentPlayerIndex}: {_choosenLine.Item1}, {_choosenLine.Item2}");
    }
}
