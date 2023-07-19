using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(Dystir.Services.LanguageService))]
namespace Dystir.Services
{
    public class LanguageService
    {
        public Action OnLanguageChanged;

        public LanguageService()
        {
            SetLanguage();
        }

        public async void LanguageChange()
        {
            string languageCode = Application.Current.Properties.FirstOrDefault(x => x.Key == "languageCode").Value?.ToString();
            languageCode = languageCode == "fo-FO" ? languageCode = "en-GB" : languageCode = "fo-FO";
            Application.Current.Properties.Remove("languageCode");
            Application.Current.Properties.Add("languageCode", languageCode);
            await Application.Current.SavePropertiesAsync();
            SetLanguage();
        }

        public void SetLanguage()
        {
            string languageCode = GetLanguageCode();
            CultureInfo cultureInfo = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Resources.Localization.Resources.Culture = cultureInfo;
            OnLanguageChanged?.Invoke();
        }

        public string GetLanguageCode()
        {
            string languageCode = Application.Current.Properties.FirstOrDefault(x => x.Key == "languageCode").Value?.ToString();
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                languageCode = "fo-FO";
            }
            languageCode = "fo-FO";
            return languageCode;
        }
    }
}
