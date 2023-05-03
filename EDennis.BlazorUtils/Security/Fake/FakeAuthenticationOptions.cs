using Microsoft.AspNetCore.Authentication;


namespace EDennis.BlazorUtils
{
    public class FakeAuthenticationOptions : AuthenticationSchemeOptions
    {
        public readonly static string ConfigurationKey = "FakeUser";
        public readonly static string AccessDefinedPath = "/Forbidden/";
        public readonly static TimeSpan CookieLifeTime = TimeSpan.FromDays(1);
        public readonly static bool CookieSlidingExpiration = true;

    }
}
