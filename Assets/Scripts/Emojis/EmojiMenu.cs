using UnityEngine;
using System.Collections;

public class EmojiMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] buttonList;
    [SerializeField] private float activationInterval = 0.5f;
    [SerializeField] private GameObject scrollUI;

    public void ButtonPopup()
    {
        StartCoroutine(ActivateButtonsWithInterval());
    }

    public void ActivateEmojiMenu() // SetActive UI that allows to scroll through emojis
    {
        scrollUI.SetActive(!scrollUI.activeSelf);
    }

    // Time interval between button activation (for animations)
    private IEnumerator ActivateButtonsWithInterval()
    {
        if (buttonList != null)
        {
            foreach (GameObject button in buttonList)
            {
                if (button != null)
                {
                    button.SetActive(!button.activeSelf);
                    yield return new WaitForSeconds(activationInterval);
                }
            }
        }
    }
}
