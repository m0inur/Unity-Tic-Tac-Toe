using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AI
{
    public class TagBoxAI : MonoBehaviour, IPointerClickHandler {
        public Image image;
        public Sprite xImg;

        private TicTacToeCreatorAI _ticTacToeCreatorAIScript;

        public bool clicked = false;
        public int boxColNum;
        public int boxRowNum;

        private void Start () {
            GameObject gameControllerObj = GameObject.Find ("Single_Player_Controller");
            image = GetComponent<Image> ();
            if (gameControllerObj != null) {
                _ticTacToeCreatorAIScript = gameControllerObj.GetComponent<TicTacToeCreatorAI> ();
            } else {
                Debug.Log ("Cannot find 'TicTacToeCreatorAI' object");
            }
        }

        public void OnPointerClick (PointerEventData eventData) {
            // If box is not yet clicked and game is not over and ai isnt making a move
            if (!clicked && !_ticTacToeCreatorAIScript.isGameOver && !_ticTacToeCreatorAIScript.isAIMoving) {
                // Put image and value on box and board
                image.sprite = xImg;
                _ticTacToeCreatorAIScript.Board[boxColNum, boxRowNum] = 1;

                _ticTacToeCreatorAIScript.boardLen++;

                if (_ticTacToeCreatorAIScript.boardLen >= _ticTacToeCreatorAIScript.grid * 2 - 1) {
                    _ticTacToeCreatorAIScript.HasEnded ();
                }

                clicked = true;
                StartCoroutine (_ticTacToeCreatorAIScript.MoveAi ());
                _ticTacToeCreatorAIScript.isAIMoving = true;
                _ticTacToeCreatorAIScript.boardLen++;
            }
        }
    }
}