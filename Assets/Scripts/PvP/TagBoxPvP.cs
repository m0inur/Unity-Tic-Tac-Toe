using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TagBoxPvP : MonoBehaviour, IPointerClickHandler {
    public Image image;
    public Sprite XImg;
    public Sprite OImg;

    private TicTacToeCreatorPvP script;

    public bool clicked = false;
    public int boxColNum;
    public int boxRowNum;

    private void Start () {
        GameObject gameControllerObj = GameObject.FindWithTag ("GameController");
        image = GetComponent<Image> ();
        if (gameControllerObj != null) {
            script = gameControllerObj.GetComponent<TicTacToeCreatorPvP> ();
        } else {
            Debug.Log ("Cannot find 'TicTacToeCreator' object");
        }
    }

    public void OnPointerClick (PointerEventData eventData) {
        // If box is not clicked or game is not yet over
        if (!clicked && !script.isGameOver) {
            if (script.P1) {
                image.sprite = XImg;
                script.board[boxColNum, boxRowNum] = 1;
            } else {
                image.sprite = OImg;
                script.board[boxColNum, boxRowNum] = 2;
            }

            clicked = true;
            script.boardLen++;
            script.ChangePlayer ();
        }
    }
}