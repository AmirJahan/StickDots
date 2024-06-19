using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    private bool isTurnActive = false;
    public Color myColor = new Color(255, 0, 0);
    public int playerIndex { get; set; }

    public string playerName = "AJ";
    public int score = 0;
    public int rank = 0;

    private const string ScoreProp = "score";
    private const string RankProp = "rank";
    private const string NameProp = "playerName";

    private void Awake()
    {
        if (photonView.IsMine)
        {
            SetPlayerProperties();
        }
    }

    private void SetPlayerProperties()
    {
        Hashtable playerProperties = new Hashtable
        {
            { ScoreProp, score },
            { RankProp, rank },
            { NameProp, playerName }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public virtual void BeginTurn()
    {
        isTurnActive = true;
        UIManager.Instance.IndicatorColorSwitch(myColor);
    }

    public virtual void EndTurn()
    {
        isTurnActive = false;
    }

    public void UpdatePlayerProperties()
    {
        if (photonView.IsMine)
        {
            Hashtable playerProperties = new Hashtable
            {
                { ScoreProp, score },
                { RankProp, rank },
                { NameProp, playerName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
    }
}
