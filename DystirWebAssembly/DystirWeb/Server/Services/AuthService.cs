using System.Linq;
using DystirWeb.Server.DystirDB;

namespace DystirWeb.Services
{
    public class AuthService
    {
        private readonly DystirService _dystirService;
        private readonly DystirDBContext _dystirDBContext;

        public AuthService(DystirService dystirService)
        {
            _dystirDBContext = dystirService.DystirDBContext;
            _dystirService = dystirService;
        }

        public bool IsAuthorized (string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                return false;
            }
            return _dystirDBContext.Administrators.Any(x => x.AdministratorToken == token);
        }

        public bool IsAuthorizedRequestor(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            var requestor = _dystirService.AllRequestor.FirstOrDefault(x => x.Name.Equals(name, System.StringComparison.CurrentCultureIgnoreCase));
            return requestor != null && requestor.Active == 1;
        }
    }
}
