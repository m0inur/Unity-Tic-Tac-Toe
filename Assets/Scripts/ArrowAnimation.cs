using System;
using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class ArrowAnimation : MonoBehaviour {
    private float _speed;
    private RectTransform _rectTrans;
    
    private void Start () {
        _speed = 0.5f;
        _rectTrans = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // If down arrow went too far down and still going down or if its too far up and its still going up
        if (_rectTrans.anchoredPosition.y <= 34 && _speed < 0|| _rectTrans.anchoredPosition.y >= 64 && _speed > 0)
        {
            _speed = -_speed;
        }
    }

    private void FixedUpdate () {
        transform.position +=  _speed * Time.deltaTime * transform.up;
    }
}