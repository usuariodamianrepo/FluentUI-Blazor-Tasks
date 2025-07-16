using Microsoft.AspNetCore.Components.Authorization;
using Shared;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FrontEnd.Blazor.Helpers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService localStorageService;
        private readonly ClaimsPrincipal anonymus = new( new ClaimsPrincipal());

        public CustomAuthenticationStateProvider(LocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var stringToken = await localStorageService.GetToken();
            if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymus));

            var deserializeToken = Serializations.DeserializeJsonString<UserSessionDTO>(stringToken);
            if (deserializeToken == null) return await Task.FromResult(new AuthenticationState(anonymus));

            var getUserClaims = DecryptToken(deserializeToken.Token!);
            if (getUserClaims == null) return await Task.FromResult(new AuthenticationState(anonymus));

            var claimsPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task UpdateAuthenticationState(UserSessionDTO userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if(userSession.Token != null || userSession.RefreshToken != null) 
            {
                var serializeSession = Serializations.SerializeObj(userSession);
                await localStorageService.SetToken(serializeSession);
                var getUserClaims = DecryptToken(userSession.Token!);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await localStorageService.RemoveToken();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private static ClaimsPrincipal SetClaimPrincipal(CustomUserClaimsDTO claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();

            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, claims.Id!),
                    new(ClaimTypes.Name, claims.Name!),
                    new(ClaimTypes.Email,claims.Email!),
                    new(ClaimTypes.Role, ClaimTypes.Role),                    
                }, "JwtAuth"));
        }

        private static CustomUserClaimsDTO DecryptToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaimsDTO();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier);
            var name = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Name);
            var email = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Email);
            var role = token.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.Role);

            return new CustomUserClaimsDTO(
                userId!.Value, 
                name!.Value, 
                email!.Value, 
                role!.Value);
        }
    }
}
