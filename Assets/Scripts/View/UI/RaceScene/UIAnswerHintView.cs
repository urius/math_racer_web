using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using View.UI.Common;

namespace View.UI.RaceScene
{
    public class UIAnswerHintView : MonoBehaviour
    {
        public event Action HintButtonClicked
        {
            add => _hintButton.ButtonClicked += value;
            remove => _hintButton.ButtonClicked -= value;
        }
        
        private const float ShowHideDurationSec = 0.3f;

        [SerializeField] private UITextButtonView _hintButton;
        [SerializeField] private TMP_Text _crystalsLeftText;

        private RectTransform _hintButtonRectTransform;
        private RectTransform _crystalsLeftTextRectTransform;
        private Vector2 _hintButtonDefaultPosition;
        private Vector2 _crystalsTextDefaultPosition;
        private Vector2 _hintButtonHidePosition;
        private Vector2 _crystalsTextHidePosition;

        private void Awake()
        {
            _hintButtonRectTransform = (RectTransform)_hintButton.transform;
            _crystalsLeftTextRectTransform = (RectTransform)_crystalsLeftText.transform;
            
            _hintButtonDefaultPosition =  _hintButtonRectTransform.anchoredPosition;
            _hintButtonHidePosition = new Vector2(500, _hintButtonDefaultPosition.y);
            _crystalsTextDefaultPosition = _crystalsLeftTextRectTransform.anchoredPosition;
            _crystalsTextHidePosition = new Vector2(_crystalsTextDefaultPosition.x, -300);

            _hintButtonRectTransform.anchoredPosition = _hintButtonHidePosition;
            _crystalsLeftTextRectTransform.anchoredPosition = _crystalsTextHidePosition;
        }

        public UniTask AnimateShowHint()
        {
            CancelTweens();
            var tcs = new UniTaskCompletionSource();

            _hintButtonRectTransform.anchoredPosition = _hintButtonHidePosition;
            _crystalsLeftTextRectTransform.anchoredPosition = _crystalsTextHidePosition;

            SetComponentsActive(true);
            
            _hintButtonRectTransform.LeanMove((Vector3)_hintButtonDefaultPosition, ShowHideDurationSec)
                .setEaseOutBack();
            _crystalsLeftTextRectTransform.LeanMove((Vector3)_crystalsTextDefaultPosition, ShowHideDurationSec)
                .setOnComplete(() => tcs.TrySetResult());

            return tcs.Task;
        }

        public UniTask AnimateHideHint()
        {
            CancelTweens();
            var tcs = new UniTaskCompletionSource();

            _hintButtonRectTransform.LeanMove((Vector3)_hintButtonHidePosition, ShowHideDurationSec);
            _crystalsLeftTextRectTransform.LeanMove((Vector3)_crystalsTextHidePosition, ShowHideDurationSec)
                .setOnComplete(() =>
                {
                    SetComponentsActive(false);
                    tcs.TrySetResult();
                });

            return tcs.Task;
        }

        private void SetComponentsActive(bool isActive)
        {
            _hintButton.gameObject.SetActive(isActive);
            _crystalsLeftText.gameObject.SetActive(isActive);
        }

        private void CancelTweens()
        {
            LeanTween.cancel(_hintButtonRectTransform.gameObject, callOnComplete: true);
            LeanTween.cancel(_crystalsLeftTextRectTransform.gameObject, callOnComplete: true);
        }

        public void SetHintButtonText(string text)
        {
            _hintButton.SetText(text);
        }

        public void SetCrystalsLeftText(string text)
        {
            _crystalsLeftText.text = text;
        }

        public void SetHintButtonInteractable(bool isInteractable)
        {
            _hintButton.SetInteractable(isInteractable);
        }
    }
}