using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class CreateRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public Text roomName;

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
            SceneManager.LoadScene("Menu");
        }
        
        public void OnClick_CreateRoom () {
            if (!PhotonNetwork.IsConnected || roomName.text == "") {
                return;
            }
            
            Debug.Log ("Creating room = " + roomName.text);
        
            RoomOptions options = new RoomOptions ();
            options.BroadcastPropsChangeToAll = true;
            options.PublishUserId = true;
            options.MaxPlayers = 2;
        
            PhotonNetwork.JoinOrCreateRoom (roomName.text, options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
        }

        // If a room has been successfully created
        public override void OnCreatedRoom () {
            Debug.Log ("Created room successfully");
            PhotonNetwork.LoadLevel(SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/WaitingRoom.unity"));
        }

        // If a room is failed to create
        public override void OnCreateRoomFailed (short returnCode, string message) {
            Debug.Log ("Room creations failed: " + message);
        }
    }
}