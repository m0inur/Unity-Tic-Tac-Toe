using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AI
{
    public class TagBoxAI : MonoBehaviour, IPointerClickHandler {
        public Image image;
        public Sprite xImg;

        public bool clicked = false;
        public int boxColNum;
        public int boxRowNum;

        private void Start () {
            image = GetComponent<Image> ();
        }

        public void OnPointerClick (PointerEventData eventData) {
            // If box is not yet clicked and game is not over and ai isnt making a move
            if (!clicked && !TicTacToeCreatorAI.Instance.isGameOver && !TicTacToeCreatorAI.Instance.isAIMoving) {
                // Put image and value on box and board
                image.sprite = xImg;
                TicTacToeCreatorAI.Instance.Board[boxColNum, boxRowNum] = 1;

                TicTacToeCreatorAI.Instance.boardLen++;

                if (TicTacToeCreatorAI.Instance.boardLen >= TicTacToeCreatorAI.Instance.grid * 2 - 1) {
                    TicTacToeCreatorAI.Instance.HasEnded ();
                }
                
                clicked = true;
                TicTacToeCreatorAI.Instance.HasEnded();
                if (!TicTacToeCreatorAI.Instance.isGameOver)
                {
                    StartCoroutine (TicTacToeCreatorAI.Instance.MoveAi ());
                }
                
                TicTacToeCreatorAI.Instance.boardLen++;
            }
        }
    }
}