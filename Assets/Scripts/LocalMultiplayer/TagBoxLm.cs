using System;
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
        private TicTacToeCreatorLm _ticTacToeCreatorScript;
        
        public bool clicked = false;
        public int boxColNum;
        public int boxRowNum;

        private void Start()
        {
            // If Tic Tac Toe creator script hasn't been initialized yet
            if (!_ticTacToeCreatorScript)
            {
                image = GetComponent<Image>();
                _ticTacToeCreatorScript = GameObject.Find("Local Multiplayer Controller").GetComponent<TicTacToeCreatorLm>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // If box is not clicked or game is not yet over
            if (!clicked && !_ticTacToeCreatorScript.isGameOver)
            {
                if (_ticTacToeCreatorScript.p1)
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
                _ticTacToeCreatorScript.ChangePlayer();
            }
        }
    }
}
