using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace My_Photon.Rooms
{
    public class CreateRoom : MonoBehaviourPunCallbacks
    {
        public GameObject waitingRoom;
        public GameObject privateMultiplayer;
        [SerializeField]
        public Text roomName;

        public void OnClick_CreateRoom () {
            if (!PhotonNetwork.IsConnected || roomName.text == "") {
                return;
            }

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            
            RoomOptions options = new RoomOptions ();
            options.BroadcastPropsChangeToAll = true;
            options.PublishUserId = true;
            options.MaxPlayers = 2;
        
            PhotonNetwork.JoinOrCreateRoom (roomName.text, options, TypedLobby.Default);
            PhotonNetwork.ConnectUsingSettings ();
        }

        // If a room has been successfully created
        public override void OnCreatedRoom ()
        {
            Debug.Log("Created Room Successfully");
            ChangeView();
        }

        // If a room is failed to create
        public override void OnCreateRoomFailed (short returnCode, string message) {
            Debug.Log ("Room creations failed: " + message);
        }

        public override void OnJoinedRoom()
        {
            ChangeView();
            Debug.Log("Joined a room");
        }
                
        private void ChangeView()
        {
            privateMultiplayer.SetActive(false);
            waitingRoom.SetActive(true);
        }
    } 
}