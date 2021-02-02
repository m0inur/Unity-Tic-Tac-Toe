using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoText : MonoBehaviour
{
    // Set reference so anyone can use it
    public static GameInfoText Instance;
    
    private Text _text;

    // Counters
    private float _maxHideTextDelay;
    private float _hideTextDelay;
    private float _textFadeDuration;

    private bool _hasDisabledText;
    
    private void Start()
    {
        Instance = this;
        
        // Set Counter Values
        _maxHideTextDelay = 5f;
        _hideTextDelay = _maxHideTextDelay;
        _textFadeDuration = 0.8f;

        _hasDisabledText = true;
        
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        // Countdown
        if (_hideTextDelay >= 0f)
        {
            _hideTextDelay -= Time.deltaTime;
        }
        else
        {
            // If countdown finished and text hasn't been disabled yet
            if (!_hasDisabledText)
            {
                Debug.Log("Hiding text");
                // Hide text
                FadeText(false);
            }
        }
    }

    public void FadeText(bool show, string message = "", bool isGood = true)
    {
        if (show)
        {
            _hasDisabledText = false;
            
            // set text to message;
            _text.text = message;
            
            // Set color
            _text.DOColor(isGood ? Color.green : Color.red, _textFadeDuration);
            
            // Reset delay
            _hideTextDelay = _maxHideTextDelay;
            
            // Show text
            _text.DOFade(1f, _textFadeDuration);
        }
        else
        {
            _hasDisabledText = true;
            _text.DOFade(0f, _textFadeDuration);
        }

    }
}
