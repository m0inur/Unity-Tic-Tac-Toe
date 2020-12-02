using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks {
    private void Update () {
        // Quit the application if back was pressed
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
    }

    public void Menu () {
        SceneManager.LoadScene ("Menu");
    }

    public void PvP () {
        SceneManager.LoadScene ("Local_Multiplayer");
    }

    public void AI () {
        SceneManager.LoadScene ("Single_Player");
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