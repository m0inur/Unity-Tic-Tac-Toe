using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace My_Photon
{
    public class TestConnect : MonoBehaviourPunCallbacks {
        // Start is called before the first frame update
        private void Start () {
            Debug.Log ("Connecting to Photon", this);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
            PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
            Connect();
        }

        private void Connect()
        {
            PhotonNetwork.ConnectUsingSettings ();
        }

        public override void OnConnectedToMaster () {
            if (!PhotonNetwork.InLobby) {
                PhotonNetwork.JoinLobby ();
            }

            Debug.Log ("Connected to Photon and joined lobby", this);
        }

        public override void OnConnected()
        {
            Debug.Log("You have connected");   
            // GameInfoText.Instance.FadeText(true, "You have connected");
        }

        public override void OnDisconnected (DisconnectCause cause) {
            if (cause.ToString() != "None")
            {
                GameInfoText.Instance.FadeText(true, "You have disconnected", false);
                PhotonNetwork.Reconnect();
            }
        }
    }
}