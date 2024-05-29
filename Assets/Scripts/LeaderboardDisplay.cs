using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{
    void Start()
    {
        LeaderboardManager.Instance.LoadScoresAsync();
    }
}
