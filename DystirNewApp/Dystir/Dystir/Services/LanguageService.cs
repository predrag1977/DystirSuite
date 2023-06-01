using System;
using System.Globalization;
using System.Linq;
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

        internal async void SetLanguage()
        {
            string languageCode = Application.Current.Properties.FirstOrDefault(x => x.Key == "languageCode").Value?.ToString();
            try
            {
                if (string.IsNullOrWhiteSpace(languageCode))
                {
                    languageCode = "en-GB";
                    Application.Current.Properties.Remove("languageCode");
                    Application.Current.Properties.Add("languageCode", languageCode);
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch
            {
                languageCode = "en-GB";
                Application.Current.Properties.Remove("languageCode");
                Application.Current.Properties.Add("languageCode", languageCode);
                await Application.Current.SavePropertiesAsync();
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
        }

        public void LanguageChange()
        {
            OnLanguageChanged?.Invoke();
        }
    }
}
