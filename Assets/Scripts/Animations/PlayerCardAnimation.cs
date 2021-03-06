﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Animations
{
    public class PlayerCardAnimation : MonoBehaviour
    {
        public static PlayerCardAnimation Instance;
        
        // Get Cards
        public Transform player1Card;
        public Transform player2Card;

        // Get Card Position properties
        public Transform player1CardOriginPos;
        public Transform player2CardOriginPos;

        public Transform player1CardTargetPos;
        public Transform player2CardTargetPos;
        
        public Text vsText;
        public Text player1CardText;

        // Duration of animation
        private float _duration;
        private float _textFadeDuration;

        // Set values
        private void Start()
        {
            Instance = this;
            
            _duration = 1;
            _textFadeDuration = 0.5f;
        }

        public void ShowPlayerCard(bool isCard1)
        {
            if (isCard1)
            {
                player1Card.position = player1CardOriginPos.position;
                player1Card.gameObject.SetActive(true);
            }
            else
            {
                player2Card.position = player2CardOriginPos.position;
                player2Card.gameObject.SetActive(true);
            }
            
            // If is card 1 show card 1 else show card 2
            var card = isCard1 ? player1Card : player2Card;
            var cardTarget = isCard1 ? player1CardTargetPos : player2CardTargetPos;

            card.DOMove(cardTarget.position, _duration);
        }

        public void ChangePlayer1Card()
        {
            player1Card.DOShakePosition(1f, 50, 100, 100);
            player1CardText.DOText("You", 0.5f);
        }

        public void HidePlayerCard(bool isCard1)
        {
            // If is card 1 show card 1 else show card 2
            var card = isCard1 ? player1Card : player2Card;
            var cardOrigin = isCard1 ? player1CardOriginPos : player2CardOriginPos;

            card.DOPause();
            card.DOMove(cardOrigin.position, _duration);
        }

        public void FadeVSText(bool show)
        {
            vsText.DOFade(show ? 1f : 0f, _textFadeDuration);
        }
    }
}