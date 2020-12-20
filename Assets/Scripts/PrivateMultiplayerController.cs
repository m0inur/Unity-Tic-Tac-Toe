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

    public void LeaveRoom()
    {
        if (buttons.activeSelf)
        {
            // Go to menu
            SceneManager.Instance.ChangeScene(gameObject.transform, menu.transform);
        }
    }

    
    public void OnClick_JoinRoom()
    {
        Debug.Log("OnClick_JoinRoom()");
        SceneManager.Instance.ChangeScene(buttons.transform, joinRoomObj.transform);
    }

    public void OnClick_CreateRoom()
    {
        SceneManager.Instance.ChangeScene(buttons.transform, gridOptionButtons.transform);
    }
}