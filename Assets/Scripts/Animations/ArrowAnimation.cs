using UnityEngine;

public class ArrowAnimation : MonoBehaviour {
    private float _speed;
    private RectTransform _rectTrans;
    
    private void Start () {
        _speed = 0.6f;
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

    private void FixedUpdate ()
    {
        var trans = transform;
        trans.position +=  _speed * Time.deltaTime * trans.up;
    }
}