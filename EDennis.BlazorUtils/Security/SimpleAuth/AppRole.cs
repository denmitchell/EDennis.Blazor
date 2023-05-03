namespace EDennis.BlazorUtils
{
    public partial class AppRole : EntityBase
    {
        public string RoleName { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }

    }
}