using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LocalMultiplayer
{
    public class TagBoxLm : MonoBehaviour, IPointerClickHandler
    {
        public static TagBoxLm Instance;
        public Image image;
        public Sprite xImg;
        public Sprite oImg;
        public TicTacToeCreatorLm ticTacToeCreatorScript;
        
        public bool clicked = false;
        public int boxColNum;
        public int boxRowNum;

        private void Start()
        {
            image = GetComponent<Image>();
            ticTacToeCreatorScript = GameObject.Find("Game Controller").GetComponent<TicTacToeCreatorLm>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // If box is not clicked or game is not yet over
            if (!clicked && !ticTacToeCreatorScript.isGameOver)
            {
                if (ticTacToeCreatorScript.p1)
                {
                    image.sprite = xImg;
                    ticTacToeCreatorScript.Board[boxColNum, boxRowNum] = 1;
                }
                else
                {
                    image.sprite = oImg;
                    ticTacToeCreatorScript.Board[boxColNum, boxRowNum] = 2;
                }

                clicked = true;
                ticTacToeCreatorScript.boardLen++;
                ticTacToeCreatorScript.ChangePlayer();
            }
        }
    }
}
