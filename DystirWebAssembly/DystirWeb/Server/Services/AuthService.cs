using System.Linq;

namespace DystirWeb.Services
{
    public class AuthService
    {
        private readonly DystirService _dystirService;

        public AuthService(DystirService dystirService)
        {
            _dystirService = dystirService;
        }

        public bool IsAuthorized (string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                return false;
            }
            return _dystirService.DystirDBContext.Administrators.Any(x => x.AdministratorToken == token);
        }
    }
}
