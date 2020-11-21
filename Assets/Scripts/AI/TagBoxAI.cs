using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TagBoxAI : MonoBehaviour, IPointerClickHandler {
    public Image image;
    public Sprite XImg;

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
        // If box is not yet clicked and game is not over and ai isnt making a move
        if (!clicked && !script.isGameOver && !script.isAIMoving) {
            // Put image and value on box and board
            image.sprite = XImg;
            script.board[boxColNum, boxRowNum] = 1;

            script.boardLen++;

            if (script.boardLen >= script.grid * 2 - 1) {
                script.HasEnded ();
            }

            clicked = true;
            StartCoroutine (script.MoveAi ());
            script.isAIMoving = true;
        }
    }
}