using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using LocalMultiplayer;
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
    public GameObject localMultiplayerButtons;

    public GameObject singlePlayer;
    public GameObject localMultiplayer;
    public GameObject privateMultiplayer;
    public GameObject multiplayer;

    private TicTacToeCreatorAI tictactoeCreatorAIScript;
    private TicTacToeCreatorLm ticTacToeCreatorLmScript;

    private void Start()
    {
        tictactoeCreatorAIScript = singlePlayer.GetComponent<TicTacToeCreatorAI>();
        ticTacToeCreatorLmScript = localMultiplayer.GetComponent<TicTacToeCreatorLm>();
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
        tictactoeImage.gameObject.SetActive(false);
        menuButtons.SetActive(false);
        localMultiplayerButtons.SetActive(true);
    }
    
    public void LocalMultiplayer3 () {
        gameObject.SetActive(false);
        ticTacToeCreatorLmScript.grid = 3;
        localMultiplayer.SetActive(true);
    }
    
    public void LocalMultiplayer4 () {
        gameObject.SetActive(false);
        ticTacToeCreatorLmScript.grid = 4;
        localMultiplayer.SetActive(true);
    }
    
    public void LocalMultiplayer5 () {
        gameObject.SetActive(false);
        ticTacToeCreatorLmScript.grid = 5;
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
        gameObject.SetActive(false);
        multiplayer.SetActive(true);
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
    
    public void LocalMultiplayerButtons_LeaveButton()
    {
        localMultiplayerButtons.SetActive(false);
        tictactoeImage.gameObject.SetActive(true);
        menuButtons.SetActive(true);
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}