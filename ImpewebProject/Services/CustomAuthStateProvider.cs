using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace ImpewebProject.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        private ClaimsPrincipal? _currentUser;

        public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser != null)
            {
                return new AuthenticationState(_currentUser);
            }

            try
            {
                var userSessionResult = await _sessionStorage.GetAsync<string>("UserRFC");
                var roleSessionResult = await _sessionStorage.GetAsync<string>("UserRole");

                var userRFC = userSessionResult.Success ? userSessionResult.Value : null;
                var userRole = roleSessionResult.Success && !string.IsNullOrEmpty(roleSessionResult.Value)
                               ? roleSessionResult.Value
                               : "Cliente";

                if (string.IsNullOrEmpty(userRFC))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userRFC),
                    new Claim(ClaimTypes.Role, userRole)
                };

                var identity = new ClaimsIdentity(claims, "CustomAuth");
                _currentUser = new ClaimsPrincipal(identity);

                return new AuthenticationState(_currentUser);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task IniciarSesion(string rfc, string rol)
        {
            await _sessionStorage.SetAsync("UserRFC", rfc);
            await _sessionStorage.SetAsync("UserRole", rol); 

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, rfc),
                new Claim(ClaimTypes.Role, rol)
            };

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public async Task CerrarSesion()
        {
            await _sessionStorage.DeleteAsync("UserRFC");
            await _sessionStorage.DeleteAsync("UserRole"); 

            _currentUser = null;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }
    }
}