using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class ArrowAnimation : MonoBehaviour {
    private float speed;
    private float changeDirecWait;

    // Start is called before the first frame update
    void Start () {
        speed = 0.5f;
        changeDirecWait = 0.5f;

        StartCoroutine (MoveDelay ());
    }

    // Update is called once per frame
    private void FixedUpdate () {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    IEnumerator MoveDelay () {
        yield return new WaitForSeconds (changeDirecWait);
        speed = -speed;

        StartCoroutine (MoveDelay ());
    }
}