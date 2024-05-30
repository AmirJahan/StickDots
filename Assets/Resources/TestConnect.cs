using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestConnect : MonoBehaviourPunCallbacks
{
    private void Awake() {
       
    }
    void Start()

    {
        print ("Connecting to server");
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon server
        //PhotonNetwork.GameVersion = "0.0.1";
        
    }

    public override void OnConnectedToMaster() //Callback function for when the first connection is established
    {
        PhotonNetwork.JoinLobby(); //Joins the Photon Lobby
        Debug.Log("Connected to Master");
    }

    public override void OnJoinedLobby() //Callback function for when the first connection is established
    {
        Debug.Log("Made it to Lobby");
    }




}