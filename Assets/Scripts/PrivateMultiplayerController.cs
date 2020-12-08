using Photon.Pun;
using UnityEngine;

public class PrivateMultiplayerController : MonoBehaviour
{
    public GameObject menu;

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
        
        menu.SetActive(true);
        gameObject.SetActive(false);
    }
}
