using System.ComponentModel.DataAnnotations;

namespace EDennis.BlazorUtils
{
    public partial class AppUser : EntityBase
    {
        [Required]
        public string UserName { get; set; }

        public int? RoleId { get; set; }

        public AppRole AppRole { get; set; }

    }
}