using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScenes : MonoBehaviour {
    public void PvP () {
        SceneManager.LoadScene ("PvP_Game");
    }

    public void AI () {
        SceneManager.LoadScene ("AI_Game");
    }

    public void MultiPlayer () {
        SceneManager.LoadScene ("MultiPlayer_Game");
    }
}