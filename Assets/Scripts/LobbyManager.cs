using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject createRoomPage;
    [SerializeField] private GameObject joinRoomPage;
    [SerializeField] private UnityEvent<Vector2> gameStart;
    [SerializeField] private TextMeshProUGUI defaultXSize;
    [SerializeField] private TextMeshProUGUI defaultYSize;
    private int x;
    private int y;

    public List<Player> playersInLobby = new List<Player>(); // Pass this list data to GameplayManager Players List

    private string roomType;
    private void Awake()
    {
        roomType = PlayerPrefs.GetString("RoomType");
        if(roomType == "Create")
        {
            createRoomPage.SetActive(true);
        }
        if (roomType == "Join")
        {
            joinRoomPage.SetActive(true);
        }
    }

    public void GameStart()
    {
        // Slider slider = createRoomPage.GetComponentInChildren<Slider>();
        x = int.Parse(defaultXSize.text);
        y = int.Parse(defaultYSize.text);

        string xSizeInput = createRoomPage.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text;
        string ySizeInput = createRoomPage.transform.GetChild(2).GetComponentInChildren<TMP_InputField>().text;

        if (xSizeInput != "")
        {
            x = int.Parse(xSizeInput);
        }

        if (ySizeInput != "")
        {
            y = int.Parse(ySizeInput);
        }

        Debug.Log(x + " " + y);

        // int boardSize = (int) slider.value;
        // TODO: 

        //return;

        gameStart.Invoke(new Vector2(x, y));
    }

    public void AddPlayer()
    {
        Player newPlayer = new Player();
        playersInLobby.Add(newPlayer);
    }
}
