using TMPro;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using Photon.Pun.UtilityScripts;

public class LeaderBoard : MonoBehaviourPunCallbacks
{

    [Header("Options")]
    public float refreshRate = 1f;

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
}

