namespace EDennis.BlazorUtils
{
    /// <summary>
    /// Example:
    /// <code>
    /// "Security": {
    ///   "IdpUserNameClaim": "preferred_username",
    ///   "SecurityTablePrefix": 3600000
    ///   "RefreshInterval": 3600000
    /// }
    /// </code>
    /// </summary>

    public class SecurityOptions
    {
        public string IdpUserNameClaim { get; set; } = "preferred_username";
        public string TablePrefix { get; set; } = "";
        public int RefreshInterval { get; set; } = 1000 * 60 * 60; //one hour
    }

}
