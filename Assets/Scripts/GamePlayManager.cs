using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour, IPunObservable
{
    [SerializeField] private int _h = 4;
    [SerializeField] private int _w = 4;
    [SerializeField] public PlayerColor[] playerColor;
    //[SerializeField] public Color[] selectedColors;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private GameObject playerContainer;
    
    //TODO: WHEN INTEGRATING COLOR PICKER
    //[SerializeField] private TextMeshProUGUI currentPlayerName;

    public Player[] players;
    public AIRandom[] randomAIs;
    public int currentPlayerIndex { get; private set; } = 0;
    public static GamePlayManager Instance { get; private set; }
    private int playerCount;
    public int AIplayerCount;
    public Board board;
    [SerializeField] private UnityEvent<Vector3> _boxCapturedEvent;
    [SerializeField] private TextMeshProUGUI _playerCountText;
    [SerializeField] private Button _gameStartButton;

    [SerializeField] private AudioClip gameOverAudioClip;
    private AudioSource audioSource;

    private PhotonView photonView;

    public int PlayersCount => playerCount;
    public int H => _h;
    public int W => _w;
    private bool isGameFinished = false;
    [SerializeField] private Timer _timer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        photonView = PhotonView.Get(this);
        audioSource = gameObject.AddComponent<AudioSource>();
        _gameStartButton.onClick.AddListener(OnClickStartGame);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("OnSceneLoaded: " + scene.name);
        //if (scene.name == "04_Local_Multiplayer" || 
        //    scene.name == "05_Multiplayer")
        //{
        //    playerCount += 1;
        //    _playerCountText.text = playerCount.ToString();
        //}
            //photonView.RPC("CreateBoardOfSize", RpcTarget.AllBufferedViaServer);
        //CreateBoardOfSize();
    }

    public void JoinRoomRPC()
    {
        photonView.RPC("GameplayJoinedRoom", RpcTarget.AllBufferedViaServer);
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("Name");
    }

    [PunRPC]
    public void GameplayJoinedRoom()
    {
        playerCount += 1;
        _playerCountText.text = playerCount.ToString();
    }

    [PunRPC]
    public void LoadLevelRPC()
    {
        PhotonNetwork.LoadLevel("04_Local_Multiplayer");
    }

    void OnClickStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //playerCount += 1;
            //PhotonNetwork.LoadLevel("04_Local_Multiplayer");
            photonView.RPC("LoadLevelRPC", RpcTarget.AllBufferedViaServer);
            photonView.RPC("CreateBoardOfSize", RpcTarget.AllBufferedViaServer);
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    UIManager.Instance.IndicatorColorSwitch(players[currentPlayerIndex].myColor);
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    EndTurn();
        //}
        if(Input.GetKeyDown(KeyCode.Q) || (board != null && board.AvailableLines.Count == 0 && !isGameFinished))
        {
            isGameFinished = true;
            UIManager.Instance.GameEndPageActive(true);

            foreach (Player player in players)
            {
                player.UpdatePlayerProperties();
            }
            
            if (_timer == null) { _timer = FindFirstObjectByType<Timer>(); }
            _timer.StopTimer();
            PlayGameOverAudio();
        }

        //if (board != null && board.AvailableLines.Count == 0 && !isGameFinished)
        //{
        //    isGameFinished = true;
        //    UIManager.Instance.GameEndPageActive(true);

        //    if (_timer == null) { _timer = FindFirstObjectByType<Timer>(); }
        //    _timer.StopTimer();
        //    PlayGameOverAudio();
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerCount);
            stream.SendNext(_h);
            stream.SendNext(_w);
            foreach(var player in playerColor)
            {
                stream.SendNext(player.myColor.r);
                stream.SendNext(player.myColor.g);
                stream.SendNext(player.myColor.b);
                stream.SendNext(player.myColor.a);
            }
        }
        else
        {
            playerCount = (int)stream.ReceiveNext();
            _h = (int)stream.ReceiveNext();
            _w = (int)stream.ReceiveNext();
            for(int i = 0; i < playerColor.Length; i++)
            {
                playerColor[i].myColor.r = (float) stream.ReceiveNext();
                playerColor[i].myColor.g = (float) stream.ReceiveNext();
                playerColor[i].myColor.b = (float) stream.ReceiveNext();
                playerColor[i].myColor.a = (float) stream.ReceiveNext();
            }
        }
    }

    public void SetBoardSize(Vector2 value)
    {
        
        _h = (int)value.y;
        _w = (int)value.x;
    }

    [PunRPC]
    public void CreateBoardOfSize()
    {
        Transform a = FindFirstObjectByType<UIManager>().transform;
        playerContainer = a.GetChild(3).gameObject;
        // if (H == 1)
        // {
        //     _h = 4; 
        //     _w = 4;
        // }
        // else if (size == 2)
        // {
        //     _h = 6;
        //     _w = 6;
        // }
        // else if (size == 3)
        // {
        //     _h = 8;
        //     _w = 8;
        // }

        InitailizePlayers();

        //TODO: WHEN INTEGRATING COLOR PICKER
        //ChangePlayerInfo();

        StartTurn();
        board = new Board(_h, _w);
        GridGenerator.Instance.CreateBoard();
        //GridGenerator.Instance.CreateBoardRPC();
        LineController.Instance.CreateLineDrawing();
    }

    void InitailizePlayers()
    {
        Debug.Log("InitailizePlayers");
        //playerCount = playerColor.Length;
        players = new Player[playerCount];
        if (players.Length == 0) { return; }

        for (int i = 0; i < playerCount - AIplayerCount; i++)
        {
            if (playerPrefab != null)
            {
                GameObject playerObject = Instantiate(playerPrefab);
                playerObject.transform.parent = playerContainer.transform;
                playerObject.name = $"{PhotonNetwork.PlayerList[i].NickName}";
                playerObject.GetComponentInChildren<TextMeshProUGUI>().text = playerObject.name;
                players[i] = playerObject.AddComponent<Player>();
                players[i].GetComponent<Player>().playerIndex = i;
                players[i].GetComponent<Player>().myColor = playerColor[i].myColor;
                //players[i].GetComponent<Player>().myColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                Debug.Log(players[i].GetComponent<Player>().myColor);
            }
        }

        for (int i = playerCount - AIplayerCount; i < playerCount; i++)
        {

            GameObject playerObject = Instantiate(playerPrefab);
            playerObject.transform.parent = playerContainer.transform;
            playerObject.name = $"player {i + 1} AI";
            playerObject.GetComponentInChildren<TextMeshProUGUI>().text = playerObject.name;
            players[i] = playerObject.AddComponent<AIRandom>();
            players[i].GetComponent<AIRandom>().playerIndex = i;
            players[i].GetComponent<AIRandom>().myColor = playerColor[i].myColor;
            //players[i].GetComponent<AIRandom>().myColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            Debug.Log(players[i].GetComponent<AIRandom>().myColor);

        }

        playerContainer.GetComponent<PlayerContainer>().InitAvatorList(playerCount);
    }
    void StartTurn()
    {
        players[currentPlayerIndex].BeginTurn();
        Timer.Instance.StartTimer();

        //TODO: WHEN INTEGRATING COLOR PICKER
        //currentPlayerName.text = players[currentPlayerIndex].playerName.ToString();
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn " + currentPlayerIndex);

        players[currentPlayerIndex].GetComponent<Player>().EndTurn();

        NextTurn();
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;

        playerContainer.GetComponent<PlayerContainer>().rotateAvator();
        StartTurn();
    }

    public void PlayersMoveRPC(Vector2 p1, Vector2 p2)
    {
        photonView.RPC("PlayersMove", RpcTarget.All, p1, p2);
    }

    [PunRPC]
    public void PlayersMove(Vector2 p1, Vector2 p2)
    {
        Debug.Log($"RPC called: PlayersMove");
        Tuple<Vector2, Vector2> lineToConnect;
        // If Vertical
        if (p1.x == p2.x)
        {
            lineToConnect = p1.y > p2.y ?
                Tuple.Create(p2, p1) : Tuple.Create(p1, p2);
        }
        else
        {
            lineToConnect = p1.x > p2.x ?
                Tuple.Create(p2, p1) : Tuple.Create(p1, p2);
        }
        int nextTurnIndex = board.MakeMove(lineToConnect, currentPlayerIndex, true);
        if (nextTurnIndex != currentPlayerIndex)
        {
            NextTurn();
        }
        else
        {
            StartTurn();
        }
    }

    public void CaptureBox(Vector3 boxCoordAndCapturedBy, int playerIndex)
    {
        players[playerIndex].score += 1;
        Debug.Log($"Captured: {boxCoordAndCapturedBy.x}, {boxCoordAndCapturedBy.y}");
        Debug.Log($"Player  {players[playerIndex].name}  Score  {players[playerIndex].score}");
        _boxCapturedEvent.Invoke(boxCoordAndCapturedBy);
    }

    private void PlayGameOverAudio()
    {
        if (gameOverAudioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverAudioClip);
        }
    }
}