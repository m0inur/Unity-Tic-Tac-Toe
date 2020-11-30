using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon
{
    public class TestConnect : MonoBehaviourPunCallbacks {
        public Text connectTxt;
        public bool _tryJoinRoom = false;

        // Start is called before the first frame update
        void Start () {
            Debug.Log ("Connecting to Photon", this);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
            PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
            PhotonNetwork.ConnectUsingSettings ();
        }

        public override void OnConnectedToMaster () {
            if (connectTxt)
            {
                connectTxt.text = "Connected";
            }

            if (!PhotonNetwork.InLobby) {
                PhotonNetwork.JoinLobby ();
                Debug.Log("Joined lobby");
            }

            Debug.Log ("Connected to Photon", this);
            _tryJoinRoom = true;
        }

        public override void OnDisconnected (DisconnectCause cause) {
            Debug.Log ("Disconnected to Photon " + cause.ToString (), this);
        }
    }
}