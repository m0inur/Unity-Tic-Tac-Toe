using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace My_Photon
{
    public class TestConnect : MonoBehaviourPunCallbacks {
        public Text connectTxt;

        // Start is called before the first frame update
        private void Start () {
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
            }

            Debug.Log ("Connected to Photon and joined lobby", this);
        }

        public override void OnDisconnected (DisconnectCause cause) {
            Debug.Log("Disconnected to photon because: " + cause.ToString());
        }
    }
}