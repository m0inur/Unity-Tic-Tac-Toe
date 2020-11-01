using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TagBox : MonoBehaviour, IPointerClickHandler {
    public Sprite XImg;
    public Sprite OImg;
    private TicTacToeCreator script;
    private bool clicked = false;
    private Image image;
    public int boxColNum;
    public int boxRowNum;

    private void Start () {
        GameObject gameControllerObj = GameObject.FindWithTag ("GameController");
        image = GetComponent<Image> ();
        if (gameControllerObj != null) {
            script = gameControllerObj.GetComponent<TicTacToeCreator> ();
        } else {
            Debug.Log ("Cannot find 'TicTacToeCreator' object");
        }
    }

    public void OnPointerClick (PointerEventData eventData) {
        // If box is not clicked or game is not yet over
        if (!clicked && !script.isGameOver) {
            if (script.P1) {
                image.sprite = XImg;
                script.values[boxColNum, boxRowNum] = 1;
            } else {
                image.sprite = OImg;
                script.values[boxColNum, boxRowNum] = 2;
            }

            script.ChangePlayer ();
            script.valuesLen++;
            script.HasMatched ();

            if (script.valuesLen >= script.grid * 2 - 1) {
                script.HasMatched ();
            }
            clicked = true;
        }
    }
}