using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scenes : MonoBehaviour {
    public void Menu () {
        SceneManager.LoadScene ("Menu");
    }

    public void PvP () {
        SceneManager.LoadScene ("PvP");
    }

    public void AI () {
        SceneManager.LoadScene ("AI");
    }

    public void MultiPlayer () {
        SceneManager.LoadScene ("MultiPlayer");
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}