using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TagBoxAI : MonoBehaviour, IPointerClickHandler {
    public Image image;
    public Sprite XImg;
    public Sprite OImg;

    private TicTacToeCreatorAI script;

    public bool clicked = false;
    public int boxColNum;
    public int boxRowNum;

    private void Start () {
        GameObject gameControllerObj = GameObject.FindWithTag ("GameController");
        image = GetComponent<Image> ();
        if (gameControllerObj != null) {
            script = gameControllerObj.GetComponent<TicTacToeCreatorAI> ();
        } else {
            Debug.Log ("Cannot find 'TicTacToeCreatorAI' object");
        }
    }

    public void OnPointerClick (PointerEventData eventData) {
        // If box is not clicked or game is not yet over
        if (!clicked && !script.isGameOver) {
            // Put image and value on box and board
            image.sprite = XImg;
            script.board[boxColNum, boxRowNum] = 1;

            script.boardLen++;
            script.ChangePlayer ();

            if (script.boardLen >= script.grid * 2 - 1) {
                int result = script.HasMatched ();

                if (result != -1) {
                    if (result == 3) {
                        script.GameOver (true);
                    } else {
                        script.GameOver (false);
                    }
                }
            }

            clicked = true;
            script.MoveAi ();
        }
    }
}