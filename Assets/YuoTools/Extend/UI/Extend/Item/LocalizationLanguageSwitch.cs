#if YuoTools
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YuoTools.UI;

public class LocalizationLanguageSwitch : MonoBehaviour
{
    void Start()
    {
        var dropDown = GetComponent<YuoHorizontalSelector>();
        var allLanguage = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in allLanguage)
        {
            YuoHorizontalSelector.SelectorItem item = dropDown.AddItem(locale.LocaleName);
            item.Action = locale;
        }

        dropDown.OnValueChanged.AddListener(item => { LocalizationSettings.SelectedLocale = (Locale)item.Action; });

        dropDown.SetItem(LocalizationSettings.SelectedLocale.LocaleName);
    }
}
#endif