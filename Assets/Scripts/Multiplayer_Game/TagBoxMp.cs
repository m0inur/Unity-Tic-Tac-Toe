using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Multiplayer_Game
{
    public class TagBoxMp : MonoBehaviourPun, IPointerClickHandler
    {
        public static TagBoxMp Instance;
        public Image image;
        public Sprite xImg;
        public Sprite oImg;
        private TicTacToeCreatorMp _ticTacToeCreatorScript;    
    
        private int _playerIndex;
        public bool clicked = false;
        public int boxColNum;
        public int boxRowNum;

        private void Start()
        {
            image = GetComponent<Image>();
            _playerIndex = PhotonNetwork.IsMasterClient ? 1 : 2;
            _ticTacToeCreatorScript = GameObject.Find("Multiplayer Game Controller").GetComponent<TicTacToeCreatorMp>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (TicTacToeCreatorMp.Instance.isMyTurn)
            {
                UseBox(_playerIndex);
                TicTacToeCreatorMp.Instance.CallRpc(boxColNum, boxRowNum);
            }
        }

        public void UseBox(int playerIndex)
        {
            // If box is not clicked or game is not yet over
            if (!clicked && !_ticTacToeCreatorScript.isGameOver)
            {
                // if (_ticTacToeCreatorScript.p1) {
                if (playerIndex == 1)
                {
                    image.sprite = xImg;
                    _ticTacToeCreatorScript.Board[boxColNum, boxRowNum] = 1;
                }
                else
                {
                    image.sprite = oImg;
                    _ticTacToeCreatorScript.Board[boxColNum, boxRowNum] = 2;
                }

                clicked = true;
                _ticTacToeCreatorScript.boardLen++;
                _ticTacToeCreatorScript.IsGameOver();
            }
        }
    }
}