using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LocalMultiplayer
{
    public class TagBoxLm : MonoBehaviour, IPointerClickHandler
    {
        public Image image;
        public Sprite xImg;
        public Sprite oImg;
        
        public bool clicked = false;
        public int boxColNum;
        public int boxRowNum;

        private void Start()
        {
            // If Tic Tac Toe creator script hasn't been initialized yet
            image = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // If box is not clicked or game is not yet over
            if (!clicked && !TicTacToeCreatorLm.Instance.isGameOver)
            {
                if (TicTacToeCreatorLm.Instance.p1)
                {
                    image.sprite = xImg;
                    TicTacToeCreatorLm.Instance.Board[boxColNum, boxRowNum] = 1;
                }
                else
                {
                    image.sprite = oImg;
                    TicTacToeCreatorLm.Instance.Board[boxColNum, boxRowNum] = 2;
                }

                clicked = true;
                TicTacToeCreatorLm.Instance.boardLen++;
                TicTacToeCreatorLm.Instance.ChangePlayer();
            }
        }
    }
}
