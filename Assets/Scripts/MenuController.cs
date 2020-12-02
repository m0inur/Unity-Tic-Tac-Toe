using System;
using System.Collections.Generic;
using My_Photon.Rooms;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks
{
    // Store scene Game Objects
    public GameObject localMultiplayer;
    public GameObject singlePlayer;
    public GameObject privateMultiplayer;
    public GameObject multiplayer;

    public Text connectionText;

    private int _roomLength;
    private bool _isConnected = false;
    private float _disableConnectionTextWait;

    private void Start()
    {
        _disableConnectionTextWait = 5f;
    }

    private void Update () {
        // Quit the application if back was pressed
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
    }

    public void Menu () {
        SceneManager.LoadScene ("Menu");
    }

    public void LocalMultiplayer () {
        gameObject.SetActive(false);
        localMultiplayer.SetActive(true);
    }

    public void SinglePlayer () {
        gameObject.SetActive(false);
        singlePlayer.SetActive(true);
    }
    
    public void Multiplayer()
    {
        // If Photon is connected
        if (_isConnected)
        {
            gameObject.SetActive(false);
            multiplayer.SetActive(true);
            multiplayer.GetComponent<MultiplayerManager>().roomLength = _roomLength;
        }
        else
        {
            ShowText("You are not connected", false);
        }
    }
    
    public void Private_Multiplayer () {
        SceneManager.LoadScene ("CreateOrJoinRoom");
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomLength = roomList.Count;
    }
    
    public override void OnConnectedToMaster ()
    {
        ShowText("You are connected", true);
        _isConnected = true;
    }

    public override void OnDisconnected (DisconnectCause cause) {
        ShowText("You are Disconnected, because: " + cause.ToString (), false);
    }

    private void ShowText(string _text, bool good)
    {
        connectionText.text = _text;
        var color = connectionText.color;

        if (good)
        {
            color = Color.green;
        }
        else
        {
            color = Color.red;
        }

        color = new Color(color.r, color.g, color.b, 1);
    }
}