using Cysharp.Threading.Tasks;
using Providers.LocalizationProvider;
using TMPro;
using UnityEngine;

namespace Utils.UITextLocalizer
{
    public class UITextLocalizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _localizationKey;
        
        private ILocalizationProvider _localizationProvider;

        private void Awake()
        {
            _localizationProvider = LocalizationsHolderSo.Instance;

            SetLocalizedText().Forget();
        }

        private bool IsLanguageSet()
        {
            return _localizationProvider.IsLanguageSet;
        }

        private async UniTaskVoid SetLocalizedText()
        {
            await UniTask.WaitUntil(IsLanguageSet);

            _text.text = _localizationProvider.GetLocale(_localizationKey);
            
            Destroy(this);
        }
    }
}