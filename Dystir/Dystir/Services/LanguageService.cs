using System;

[assembly: Dependency(typeof(Dystir.Services.LanguageService))]
namespace Dystir.Services
{
    public class LanguageService
    {
        public Action OnLanguageChanged;

        public LanguageService()
        {
            
        }

        public void LanguageChange()
        {
            OnLanguageChanged?.Invoke();
        }
    }
}
