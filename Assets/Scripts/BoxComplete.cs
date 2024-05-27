using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum PlayerColour
{
    player_blue,
    player_red
}

public class BoxComplete : MonoBehaviour
{
    private List<GameObject> boxBackgrounds = new List<GameObject>();

    private Material activeColor;
    [SerializeField] private Shader completionShader;
    [SerializeField] private AudioClip audioClip;

    private int col;
    private int winPlayer;

    private Queue<Vector3> boxesToAnimate = new Queue<Vector3>();

    //[SerializeField] Player winPlayer;
    //[SerializeField] int col;
    //[SerializeField] int x, y;
    private bool blingMode;

    public List<GameObject> BoxBackgrounds
    {
        get { return boxBackgrounds; }
        set { boxBackgrounds = value; }
    }

    private void Start()
    {
        // Width in GameManager stores num of dots
        // For box need to -1
        col = GamePlayManager.Instance.W - 1;
        blingMode = false;
        ResetBling();
    }

    int getBoxHash(int x, int y)
    {
        // hash code for coord
        return x + (y * (col));
    }

    public void PlayCaptureBoxAnim(Vector3 boxCoordAndCapturedBy)
    {
        int x = (int)boxCoordAndCapturedBy.x;
        int y = (int)boxCoordAndCapturedBy.y;
        winPlayer = (int)boxCoordAndCapturedBy.z;

        // Add the box coordinate to the queue
        boxesToAnimate.Enqueue(boxCoordAndCapturedBy);

        // If this is the first box to animate, start the animation coroutine
        if (boxesToAnimate.Count == 1)
        {
            float delayBetweenAnimations = 0.5f;
            StartCoroutine(AnimateNextBoxAfterDelay(delayBetweenAnimations));
        }
    }

    private IEnumerator AnimateNextBoxAfterDelay(float delay)
    {
        while (boxesToAnimate.Count > 0)
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delay);

            // Get the next box to animate
            Vector3 nextBoxCoord = boxesToAnimate.Dequeue();

            // Get the specific gameobject renderer by hash code
            Renderer ren = boxBackgrounds[getBoxHash((int)nextBoxCoord.x, (int)nextBoxCoord.y)].gameObject.GetComponent<Renderer>();

            // Activate the game object
            ren.gameObject.SetActive(true);

            // Set the active color for animation
            activeColor = new Material(completionShader);
            activeColor.SetColor("_backgroundColor", GamePlayManager.Instance.players[winPlayer].myColor);
            ren.material = activeColor;
            blingMode = true;

        }
    }

    private void ResetBling()
    {
        blingMode = false;
    }

    private void Update()
    {
        if (blingMode)
        {
            float offset = activeColor.GetFloat("_HighLightOffset");
            activeColor.SetFloat("_HighLightOffset", offset + Time.deltaTime / 3.0f);
            if (offset >= 1.0f) ResetBling();
        }
    }
}