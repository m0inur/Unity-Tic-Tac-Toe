using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoText : MonoBehaviour
{
    // Set reference so anyone can use it
    public static GameInfoText Instance;
    
    [HideInInspector]
    public Text text;

    // Counters
    private float _maxHideTextDelay;
    private float _hideTextDelay;
    private float _textFadeDuration;

    private bool _hasStoppedCoroutine;
    
    private void Start()
    {
        Instance = this;
        
        // Set Counter Values
        _maxHideTextDelay = 5f;
        _hideTextDelay = _maxHideTextDelay;
        _textFadeDuration = 0.5f;
        
        text = GetComponent<Text>();

        _hasStoppedCoroutine = false;
    }

    public void FadeText(bool show, bool isGood = true)
    {
        if (show)
        {
            text.DOColor(isGood ? Color.green : Color.red, _textFadeDuration);
            
            // Give text more time
            _hasStoppedCoroutine = true;
            StopCoroutine(HideText());
            StartCoroutine(HideText());
        }

        text.DOFade(show ? 1f : 0f, _textFadeDuration);
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(_hideTextDelay);
        
        // If coroutine hasn't been stopped
        if (!_hasStoppedCoroutine)
        {
            FadeText(false);
        }
        
        _hasStoppedCoroutine = false;
    }
}
