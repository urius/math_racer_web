using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Providers.LocalizationProvider
{
    [CreateAssetMenu(fileName = "LocalizationsHolderSo", menuName = "ScriptableObjects/LocalizationsHolderSo")]
    public class LocalizationsHolderSo : ScriptableObject, ILocalizationProvider
    {
        public static ILocalizationProvider Instance { get; private set; }
        
        [SerializeField] private TextAsset _localizationsJson;

        private string _localeLang = null;
        private Dictionary<string, LocalizationItemData> _localizationByKey;

        public bool IsLanguageSet => _localeLang != null;

        private void OnEnable()
        {
            var localizationItems = JsonUtility.FromJson<LocalizationsData>(_localizationsJson.text);
            _localizationByKey = localizationItems.localizations.ToDictionary(d => d.key);

            Instance = this;
        }

        public string GetLocale(string key)
        {
            return _localizationByKey.TryGetValue(key, out var localizationItemData)
                ? GetLocalizationFromItem(localizationItemData)
                : key;
        }

        public void SetLocaleLang(string lang)
        {
            _localeLang = lang.ToLower();
        }

        private string GetLocalizationFromItem(LocalizationItemData localizationItem)
        {
            return _localeLang switch
            {
                "ru" => localizationItem.ru,
                _ => localizationItem.en
            };
        }
    }

    public interface ILocalizationProvider
    {
        public bool IsLanguageSet { get; }
        
        public string GetLocale(string key);
    }
}