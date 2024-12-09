using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using View.Extensions;
using View.UI.Common;

namespace View.UI.NewLevelScene
{
    public class UINewLevelSceneRootCanvasView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText1;
        [SerializeField] private TMP_Text _levelText2;
        [SerializeField] private RectTransform _levelProgressTransform;
        [SerializeField] private TMP_Text _expAmountText;
        [SerializeField] private ParticleSystem _starsParticleSystem;
        
        [SerializeField] private TMP_Text _congratsText;
        
        [SerializeField] private TMP_Text _newCarsText;
        [SerializeField] private RectTransform _newCarIconsContainerTransform;
        [SerializeField] private GameObject _newCarIconPrefab;
        
        [SerializeField] private UITextButtonView _continueButton;

        private readonly List<UINewLevelSceneNewCarIconView> _carIconViews = new();
        
        private Vector2 _continueButtonTargetPosition;

        public IReadOnlyList<UINewLevelSceneNewCarIconView> CarIconViews => _carIconViews;
        public UITextButtonView ContinueButton => _continueButton;

        private void Awake()
        {
            _congratsText.SetAlpha(0);
            _newCarsText.SetAlpha(0);

            _continueButtonTargetPosition = _continueButton.RectTransform.anchoredPosition;
            _continueButton.RectTransform.anchoredPosition = new Vector2(_continueButtonTargetPosition.x, -100);
        }

        public void ShowStarParticles()
        {
            _starsParticleSystem.Play();
        }
        
        public async UniTask AnimateCongratsTextAppear()
        {
            await _congratsText.AnimateAlpha(1, 0.5f);
        }
        
        public async UniTask AnimateNewCarsTextAppear()
        {
            await _newCarsText.AnimateAlpha(1, 0.5f);
        }

        public void AddCarIcon(Sprite carIconSprite)
        {
            var carIconGo = Instantiate(_newCarIconPrefab, _newCarIconsContainerTransform);
            var carIconView = carIconGo.GetComponent<UINewLevelSceneNewCarIconView>();
            carIconView.SetSprite(carIconSprite);
            _carIconViews.Add(carIconView);

            UpdateCarIconPositions();
        }

        public void SetExpProgressLineXScale(float xScale)
        {
            var scale = _levelProgressTransform.localScale;
            scale.x = xScale;
            _levelProgressTransform.localScale = scale;
        }

        public void SetLevelText1(string text)
        {
            _levelText1.text = text;
        }

        public void SetLevelText2(string text)
        {
            _levelText2.text = text;
        }

        public void SetExpAmountText(string text)
        {
            _expAmountText.text = text;
        }

        public void SetCongratsText(string text)
        {
            _congratsText.text = text;
        }

        public void SetNewCarsText(string text)
        {
            _newCarsText.text = text;
        }

        public void AnimateContinueButtonAppear()
        {
            _continueButton.RectTransform
                .LeanMove((Vector3)_continueButtonTargetPosition, 0.6f)
                .setEaseOutBack();
        }

        private void UpdateCarIconPositions()
        {
            if (_carIconViews.Count <= 0) return;

            for (var i = 0; i < _carIconViews.Count; i++)
            {
                var rectTransform = _carIconViews[i].RectTransform;
                var xAnchor = (float)(i + 1) / (_carIconViews.Count + 1);
                rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(xAnchor, 0.5f);
            }
        }
    }
}