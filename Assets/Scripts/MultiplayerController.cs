using System;
using My_Photon.Rooms;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerController : MonoBehaviour
{
    public GameObject menu;
    public GameObject gridOptionButtons;
    public GameObject multiplayerManager;
    public GameObject multiplayerGame;

    public Button gridButton1;
    public Button gridButton2;
    public Button gridButton3;

    private void Start()
    {
        gridButton1.onClick.AddListener(delegate {OnClick_InitMultiplayer(3); });
        gridButton2.onClick.AddListener(delegate {OnClick_InitMultiplayer(4); });
        gridButton3.onClick.AddListener(delegate {OnClick_InitMultiplayer(5); });
    }

    private void OnDisable()
    {
        gridOptionButtons.SetActive(true);
        multiplayerManager.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        // if back was pressed leave room
        if (Input.GetKeyDown (KeyCode.Escape))
        {
            if (gridOptionButtons.active)
            {
                OnClick_GOBLeave();
            }
            else if(multiplayerManager.active)
            {
                OnClick_MPLeave();
            }
        }
    }
    
    // Go back to menu
    public void OnClick_GOBLeave()
    {
        SceneManager.Instance.ChangeScene(transform, menu.transform);
    }
        
    public void OnClick_MPLeave()
    {
        SceneManager.Instance.ChangeScene(multiplayerManager.transform, gridOptionButtons.transform);
        
    }

    private void OnClick_InitMultiplayer(int grid)
    {
        Debug.Log("Grid = " + grid);
        // Set grid value
        multiplayerManager.GetComponent<MultiplayerManager>().grid = grid;
        SceneManager.Instance.ChangeScene(gridOptionButtons.transform, multiplayerManager.transform);
    }
}
