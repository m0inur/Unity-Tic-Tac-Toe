using AI;
using LocalMultiplayer;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks
{
    // Store scene Game Objects
    public Transform mainMenu;
    public GameObject singlePlayerButtons;
    public GameObject localMultiplayerButtons;

    public GameObject singlePlayer;
    public GameObject localMultiplayer;
    public GameObject privateMultiplayer;
    public GameObject multiplayer;

    public Button lMGridButton1;
    public Button lMGridButton2;
    public Button lMGridButton3;
    
    public Button easyModeButton;
    public Button mediumModeButton;
    public Button hardModeButton;
    
    private TicTacToeCreatorAI _ticTacToeCreatorAIScript;
    private TicTacToeCreatorLm _ticTacToeCreatorLmScript;

    private void Start()
    {
        _ticTacToeCreatorAIScript = singlePlayer.GetComponent<TicTacToeCreatorAI>();
        _ticTacToeCreatorLmScript = localMultiplayer.GetComponent<TicTacToeCreatorLm>();
        
        // Set button listeners
        lMGridButton1.onClick.AddListener(delegate { LocalMultiplayer(3); });
        lMGridButton2.onClick.AddListener(delegate { LocalMultiplayer(4); });
        lMGridButton3.onClick.AddListener(delegate { LocalMultiplayer(5); });
        
        easyModeButton.onClick.AddListener(delegate { SinglePlayer(0); });
        mediumModeButton.onClick.AddListener(delegate { SinglePlayer(1); });
        hardModeButton.onClick.AddListener(delegate { SinglePlayer(2); });
    }

    private void Update () {
        // Quit the application if back was pressed
        if (Input.GetKeyDown (KeyCode.Escape)) {
            if (mainMenu.gameObject.activeInHierarchy)
            {
                Application.Quit ();
            } else if (localMultiplayerButtons.gameObject.activeInHierarchy)
            {
                LocalMultiplayerButtons_LeaveButton();
            }
            else if(singlePlayerButtons.gameObject.activeInHierarchy)
            {
                SinglePlayerButtons_LeaveButton();
            }
        }
    }

    private void LocalMultiplayer (int gridVal) {
        _ticTacToeCreatorLmScript.grid = gridVal;
        SceneManager.Instance.ChangeScene(transform, localMultiplayer.transform);
    }
    
    private void SinglePlayer(int modeVal)
    {
        Debug.Log("Bot Mode = " + modeVal);
        _ticTacToeCreatorLmScript.grid = 3;
        _ticTacToeCreatorAIScript.mode = modeVal;
        _ticTacToeCreatorAIScript.isFakeMp = false;
        SceneManager.Instance.ChangeScene(transform, singlePlayer.transform);
    }
    
    public void LocalMultiplayerButtons () {
        // Show local multiplayer buttons
        SceneManager.Instance.ChangeScene(mainMenu, localMultiplayerButtons.transform);
    }
    
    public void SinglePlayerButtons () {
        // Show single player buttons
        SceneManager.Instance.ChangeScene(mainMenu, singlePlayerButtons.transform);
    }
    
    public void Multiplayer()
    {
        SceneManager.Instance.ChangeScene(transform, multiplayer.transform);
    }
    
    public void Private_Multiplayer () {
        SceneManager.Instance.ChangeScene(transform, privateMultiplayer.transform);
    }

    public void SinglePlayerButtons_LeaveButton()
    {
        SceneManager.Instance.ChangeScene(singlePlayerButtons.transform, mainMenu);
    }
    
    public void LocalMultiplayerButtons_LeaveButton()
    {
        SceneManager.Instance.ChangeScene(localMultiplayerButtons.transform, mainMenu);
    }
}