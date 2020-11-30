using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks {
    public Image particlePr;

    private Image _particle;
    private float _particleCount;
    private float _particleW;
    private float _particleH;
    private float _randParticleSize;

    private Canvas _canvas;
    private float _canvasW;
    private float _canvasH;

    // particlet is called before the first frame update
    void Start () {
        _canvas = FindObjectOfType<Canvas> ();
        _canvasW = _canvas.GetComponent<RectTransform> ().rect.width;
        _canvasH = _canvas.GetComponent<RectTransform> ().rect.height;

        _particleW = particlePr.GetComponent<RectTransform> ().rect.width;
        _particleH = particlePr.GetComponent<RectTransform> ().rect.height;

        _particleCount = 250;
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
        for (var i = 0; i <= _particleCount; i++) {
            _particle = Instantiate (particlePr, new Vector3 (UnityEngine.Random.Range (_particleW, _canvasW), -(UnityEngine.Random.Range (_particleH, _canvasH)), 0), Quaternion.identity);
            _particle.transform.SetParent (GameObject.Find ("Particle Spawner").transform, false);

            // Randomize Opacity
            Image image = _particle.GetComponent<Image> ();
            image.color = new Color (image.color.r, image.color.g, image.color.b, UnityEngine.Random.Range (0.1f, 0.4f));

            // Randomize particle sizes
            _randParticleSize = UnityEngine.Random.Range (_particleW, _particleW + 5);
            _particle.GetComponent<RectTransform> ().sizeDelta = new Vector2 (_randParticleSize, _randParticleSize);
        }
    }
    #endregion

    public void Menu () {
        SceneManager.LoadScene ("Menu");
    }

    public void PvP () {
        SceneManager.LoadScene ("Local_Multiplayer");
    }

    public void AI () {
        SceneManager.LoadScene ("Single_Player");
    }
    
    public void Private_Multiplayer_Rooms () {
        SceneManager.LoadScene ("CreateOrJoinRoom");
    }

    public void Multiplayer_Rooom()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void PlayAgain () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
}