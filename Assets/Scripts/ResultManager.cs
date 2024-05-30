using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public ResultScoreView scoreViewPrefab;
    public Transform scoresContainer;

    private List<Player> players;
    //[SerializeField] private LeaderboardScoreView leaderboardScoreView;
    //[SerializeField] private Transform container;

    //private void OnEnable()
    //{
    //    //LeaderboardManager.Instance.LoadScoresAsync();
    //}
    private void Start()
    {
        
        players = GamePlayManager.Instance.players.ToList();

        SortPlayersByScore();

        foreach (Player player in players)
        {
            var scoreView = Instantiate(scoreViewPrefab, scoresContainer);
            scoreView.Initialize(player.rank.ToString(), player.name,
            player.score.ToString());
        }
    }

    public void SortPlayersByScore()
    {
        // Sort players by score in descending order
        players = players.OrderByDescending(player => player.score).ToList();

        // Update ranks based on sorted order
        for (int i = 0; i < players.Count; i++)
        {
            players[i].rank = i + 1;
        }
    }

}
