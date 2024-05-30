using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using GameEvents;
using UnityEngine.Assertions.Must;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image uiFill;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] private BoolEventAsset _playBeepSound;
    [SerializeField] public float timeRemaining;
    [SerializeField] private float originalTime;
    [SerializeField] private int TimeForEachTurn;
    [SerializeField] private bool isRunning = false;
    public static Timer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (isRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                GamePlayManager.Instance.NextTurn();
            }

            if (Mathf.Abs(timeRemaining - 5.0f) < 0.01f)
            {
                _playBeepSound.Invoke(true);
            }

            if (timeRemaining <= 3)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }
    public void StartTimer()
    {
        int seconds = TimeForEachTurn;
        timeRemaining = seconds + 1;
        originalTime = timeRemaining;
        isRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        uiFill.fillAmount = Mathf.InverseLerp(0, originalTime, timeRemaining);
    }
}
