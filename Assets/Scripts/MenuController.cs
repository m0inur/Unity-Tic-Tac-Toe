using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public Image particlePr;

    private Image particle;
    private float particleCount;
    private float particleW;
    private float particleH;
    private float randParticleSize;

    private Canvas canvas;
    private float canvasW;
    private float canvasH;

    // particlet is called before the first frame update
    void Start () {
        canvas = FindObjectOfType<Canvas> ();
        canvasW = canvas.GetComponent<RectTransform> ().rect.width;
        canvasH = canvas.GetComponent<RectTransform> ().rect.height;

        particleW = particlePr.GetComponent<RectTransform> ().rect.width;
        particleH = particlePr.GetComponent<RectTransform> ().rect.height;

        particleCount = 250;
        InitParticles ();
    }

    private void Update () {
        // Quit the application if back was pressed
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
    }

    #region Init particles
    public void InitParticles () {
        for (var i = 0; i <= particleCount; i++) {
            particle = Instantiate (particlePr, new Vector3 (UnityEngine.Random.Range (particleW, canvasW), -(UnityEngine.Random.Range (particleH, canvasH)), 0), Quaternion.identity);
            particle.transform.SetParent (GameObject.Find ("Particle Spawner").transform, false);

            // Randomize Opacity
            Image image = particle.GetComponent<Image> ();
            image.color = new Color (image.color.r, image.color.g, image.color.b, UnityEngine.Random.Range (0.1f, 0.4f));

            // Randomize particle sizes
            randParticleSize = UnityEngine.Random.Range (particleW, particleW + 5);
            particle.GetComponent<RectTransform> ().sizeDelta = new Vector2 (randParticleSize, randParticleSize);
        }
    }
    #endregion

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