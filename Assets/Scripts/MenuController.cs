using System;
using System.Collections;
using System.Collections.Generic;
using AI;
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
    public Image tictactoeImage;
    
    public GameObject menuButtons;
    public GameObject singlePlayerButtons;
    public GameObject localMultiplayer;
    public GameObject singlePlayer;
    public GameObject privateMultiplayer;
    public GameObject multiplayer;

    public Text connectionText;

    private IEnumerator _currentCoroutine;
    
    private string _quickmatchRoomName;

    private TicTacToeCreatorAI tictactoeCreatorAIScript;
    private int _roomLength;
    private bool _isConnected = false;
    private float _disableConnectionTextWait;
    
    private void Start()
    {
        _disableConnectionTextWait = 3f;
        tictactoeCreatorAIScript = singlePlayer.GetComponent<TicTacToeCreatorAI>();
        _quickmatchRoomName = "PunfishQuickmatch2131";
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
        tictactoeImage.gameObject.SetActive(false);
        menuButtons.SetActive(false);
        singlePlayerButtons.SetActive(true);
    }

    public void EasyBot()
    {
        gameObject.SetActive(false);
        tictactoeCreatorAIScript.mode = 0;
        singlePlayer.SetActive(true);
    }
    
    public void MediumBot()
    {
        gameObject.SetActive(false);
        tictactoeCreatorAIScript.mode = 1;
        singlePlayer.SetActive(true);
    }
    
    public void HardBot()
    {
        gameObject.SetActive(false);
        tictactoeCreatorAIScript.mode = 2;
        singlePlayer.SetActive(true);
    }
    
    public void Multiplayer()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        
        // If Photon is connected
        if (_isConnected)
        {
            gameObject.SetActive(false);
            multiplayer.SetActive(true);
            multiplayer.GetComponent<MultiplayerManager>().roomLength = _roomLength;
        }
        else
        {
            StartCoroutine(ShowText("You are not connected", false));
        }
    }
    
    public void Private_Multiplayer () {
        gameObject.SetActive(false);
        privateMultiplayer.SetActive(true);
    }

    public void SinglePlayerButtons_LeaveButton()
    {
        singlePlayerButtons.SetActive(false);
        tictactoeImage.gameObject.SetActive(true);
        menuButtons.SetActive(true);
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomLength = roomList.Count;
        
        foreach (var info in roomList)
        {
            if (info.Name != _quickmatchRoomName)
            {
                _roomLength--;
            }
        }
    }
    
    public override void OnConnectedToMaster ()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        
        _isConnected = true;
    }

    public override void OnDisconnected (DisconnectCause cause)
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        
        _currentCoroutine = ShowText("You are Disconnected, because: " + cause.ToString(), false);
        StartCoroutine(_currentCoroutine);
    }

    private IEnumerator ShowText(string _text, bool isGood)
    {
        Color color;
        connectionText.text = _text;

        if (isGood)
        {
            Debug.Log("Setting color to green");
            color = Color.green;
        }
        else
        {
            Debug.Log("Setting color to red");
            color = Color.red;
        }
        
        Debug.Log("Show");
        // Show
        for (float i = 0; i <= 5; i += Time.deltaTime)
        { 
            // set color with i as alpha
            connectionText.color = new Color(color.r, color.g, color.b, i);
            yield return null;
        }
            
        yield return new WaitForSeconds(_disableConnectionTextWait);
        
        Debug.Log("Hide");
        // Hide
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            connectionText.color = new Color(color.r, color.g, color.b, i);
            yield return null;
        }
    }
}