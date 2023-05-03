using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace EDennis.BlazorUtils
{
    /// <summary>
    /// NOTE: This fake authentication handler is used for testing, and it is designed to
    /// require very simple configuration.  Ensure that you set a command-line argument 
    /// of FakeUser=Maria (or some other user)  
    /// </summary>
    public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationOptions>
        //IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly SecurityOptions _securityOptions;


        public FakeAuthenticationHandler(IOptionsMonitor<FakeAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            IConfiguration config, IOptionsMonitor<SecurityOptions> securityOptions
            ) : base(options, logger, encoder, clock)
        {
            _config = config;
            _securityOptions = securityOptions.CurrentValue;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme)
        {
            return await HandleAuthenticateAsync();
        }


        //protected override async Task<AuthenticateResult> HandleAuthenticateAsync()


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userNameClaim = _config.GetValue<string>(FakeAuthenticationOptions.ConfigurationKey);
            if (string.IsNullOrEmpty(userNameClaim))
                throw new ArgumentException($"Invalid configuration value for {FakeAuthenticationOptions.ConfigurationKey}");

            var claims = new Claim[] {
                new Claim(_securityOptions.IdpUserNameClaim, userNameClaim),
                new Claim(ClaimTypes.Name, userNameClaim)
            };


            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(FakeAuthenticationHandler)));

            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }


    }

    /*
    public class FakeAuthenticationSignoutHandler : IAuthenticationSignInHandler
    {

        private ClaimsPrincipal _user;

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(_user,AuthenticationSchemeConstants.FakeAuthenticationScheme)));
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            return Task.CompletedTask;
        }

        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            _user = user;
            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            _user = null;
            return Task.CompletedTask;
        }
    }
    */
}
