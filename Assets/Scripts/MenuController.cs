using System;
using System.Collections.Generic;
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
    
    public void Private_Multiplayer_Rooms () {
        SceneManager.LoadScene ("CreateOrJoinRoom");
    }

    public void Multiplayer_Rooom()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}