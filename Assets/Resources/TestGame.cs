using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TestGame : MonoBehaviourPunCallbacks
{
    public Button button1;  // Drag the 'Player 1 btn' in Unity Inspector
    public Button button2;  // Drag the 'Player 2 btn' in Unity Inspector
    public Button button3;  // Drag the 'Player 3 btn' in Unity Inspector

    private PhotonView photonView;

    void Start()
    {
        photonView = PhotonView.Get(this); // Ensure this GameObject has a PhotonView component.
        HookUpButtonEvents();
    }

    private void HookUpButtonEvents()
    {
        button1.onClick.AddListener(OnButton1Clicked);
        button2.onClick.AddListener(OnButton2Clicked);
        button3.onClick.AddListener(OnButton3Clicked);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void OnButton1Clicked()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            ChangeButtonColor(button1, Color.red);
        }
    }

    public void OnButton2Clicked()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            ChangeButtonColor(button2, Color.green);
        }
    }

    public void OnButton3Clicked()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 3)
        {
            ChangeButtonColor(button3, Color.blue);
        }
    }

    void ChangeButtonColor(Button button, Color color)
    {
        Debug.Log($"Attempting to change color of {button.gameObject.name} to {color}");
        photonView.RPC("ChangeColor", RpcTarget.AllBuffered, button.gameObject.name, color.r, color.g, color.b);
    }

    [PunRPC]
    void ChangeColor(string buttonName, float r, float g, float b)
    {
        Debug.Log($"RPC called: Changing color of {buttonName} to ({r}, {g}, {b})");
        Button button = FindButtonByName(buttonName);
        if (button != null)
        {
            button.GetComponent<Image>().color = new Color(r, g, b);
        }
        else
        {
            Debug.Log("Button not found with the name: " + buttonName);
        }
    }

    private Button FindButtonByName(string name)
    {
        switch (name)
        {
            case "Player 1 btn": return button1;
            case "Player 2 btn": return button2;
            case "Player 3 btn": return button3;
            default:
                Debug.LogError("Button name does not match any predefined names.");
                return null;
        }
    }
}
