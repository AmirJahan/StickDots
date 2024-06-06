using TMPro;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using Photon.Pun.UtilityScripts;
using System.Collections;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LeaderBoard : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public LeaderboardScoreView PlayerScoreView;
    public Transform Container;

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {

        var sortedPlayerList=(from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        
        int i = 0;

        foreach (var player in sortedPlayerList)
        {
            Instantiate(PlayerScoreView, Container);
            
            if (player.NickName == "")
            {
                player.NickName = "unnamed";
            }
            PlayerScoreView.Initialize((i + 1).ToString(),player.NickName, player.GetScore().ToString());

            i++;
        }
    }

    public void UpdatePlayerScore(Player player, int newScore)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "score", newScore }
        };
        //player.SetCustomProperties(props);
    }

    public void OnPlayerScoreChanged(Player player, int newScore)
    {
        UpdatePlayerScore(player, newScore);
        Refresh();
    }
}

