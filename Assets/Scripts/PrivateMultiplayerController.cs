using System;
using Photon.Pun;
using UnityEngine;

public class PrivateMultiplayerController : MonoBehaviour
{
    public GameObject menu;
    public GameObject joinRoomObj;
    
    public GameObject buttons;
    public GameObject gridOptionButtons;

    private void Update () {
        // if back was pressed leave room
        if (Input.GetKeyDown (KeyCode.Escape))
        {
            LeaveRoom();
        }
    }

    // Go to menu
    public void LeaveRoom()
    {          
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving photon room");
            PhotonNetwork.LeaveRoom();
        }

        // If PM Buttons are active go to menu
        if (buttons.gameObject.active)
        {
            gameObject.SetActive(false);
            menu.SetActive(true);
        } else if (gridOptionButtons.gameObject.active)
        {
            // If grid option buttons are active deactivate and activate PM Buttons
            gridOptionButtons.SetActive(false);
            buttons.SetActive(true);
        } else if (joinRoomObj.gameObject.active)
        {
            // If join room is active deactivate and activate PM Buttons
            joinRoomObj.gameObject.SetActive(false);
            buttons.SetActive(true);
        }
    }

    public void OnClick_JoinRoom()
    {
        Debug.Log("OnClick_JoinRoom()");
        buttons.SetActive(false);
        joinRoomObj.SetActive(true);
    }

    public void OnClick_CreateRoom()
    {
        buttons.SetActive(false);
        gridOptionButtons.SetActive(true);
    }
}
