using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MpTextAnimation : MonoBehaviour
{
    private Text _text;
    private string _orgText;
    private int _textLen;
    
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        _orgText = _text.text;
        _textLen = _text.text.Length;
        
        StartCoroutine(TextAnimation());
    }

    private IEnumerator TextAnimation()
    {
        for (var i = 0; i <= 3; i++)
        {
            _text.text = _text.text.Substring(0, _textLen - i);
            yield return new WaitForSeconds(0.2f);
        }
        
        _text.text = _orgText;
        StartCoroutine(TextAnimation());
    }
}
