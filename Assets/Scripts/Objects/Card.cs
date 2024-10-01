using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Managers;

namespace Objects
{
    public class Card : MonoBehaviour
    {
        public int tileID;
        public Image cardImage;
        public Image innerImage;
        private Button cardButton;
        
        public Sprite frontSprite;
        public Sprite backSprite;

        private bool isFlipped = false;
        private bool isMatched = false;
        private Transform cardTransform;

        void Awake()
        {
            cardTransform = transform;
            cardImage.sprite = backSprite;
            cardButton = GetComponent<Button>();
            cardButton.onClick.AddListener(OnCardClicked);
        }

        public void Initialize(TileData tileData)
        {
            tileID = tileData.tileID;
            innerImage.sprite = tileData.tileSprite;

            innerImage.gameObject.SetActive(false);
            isFlipped = false;
            isMatched = false;
        }

        public Tween FlipCard(float duration = 0.5f)
        {
            if (isFlipped) return null;
        
            isFlipped = true;
        
            Sequence flipSequence = DOTween.Sequence();
        
            flipSequence.Append(cardTransform.DORotate(new Vector3(0, 90, 0), duration / 2))
                .AppendCallback(ChangeCardFace)
                .Append(cardTransform.DORotate(Vector3.zero, duration / 2));
        
            return flipSequence;
        }

        private void ChangeCardFace()
        {
            if (cardImage.sprite == backSprite)
            {
                cardImage.sprite = frontSprite;
                innerImage.gameObject.SetActive(true);
            }
            else
            {
                cardImage.sprite = backSprite;
                innerImage.gameObject.SetActive(false);
            }
        }
        

        private void OnCardClicked()
        {
            LevelManager.Instance.OnCardClicked(this);
        }

        public void FlipBackCard(float duration = 0.5f)
        {
            if (!isFlipped) return;
        
            Sequence flipSequence = DOTween.Sequence();
        
            flipSequence.Append(cardTransform.DORotate(new Vector3(0, 90, 0), duration / 2))
                .AppendCallback(ChangeCardFace)
                .Append(cardTransform.DORotate(Vector3.zero, duration / 2))
                .OnComplete(() => isFlipped = false);
        }

        public void AppearCard(float duration = 0.5f)
        {
            cardTransform.localScale = Vector3.zero;
            cardTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce);
        }

        public void DisappearCard(float duration = 0.5f)
        {
            cardTransform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack);
        }

        public void ResetCard()
        {
            cardTransform.localRotation = Quaternion.identity;
            cardTransform.localScale = Vector3.one;
            cardImage.sprite = backSprite;
            isFlipped = false;
            innerImage.gameObject.SetActive(false);
        }

        public bool IsFlipped()
        {
            return isFlipped;
        }

        public bool IsMatching(Card otherCard)
        {
            return this.tileID == otherCard.tileID;
        }
        
        public void MarkAsMatched()
        {
            isMatched = true;
        }

        public bool IsMatched()
        {
            return isMatched;
        }
    }
}
