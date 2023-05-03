using EDennis.BlazorUtils;
using System.ComponentModel.DataAnnotations;

namespace EDennis.BlazorHits
{
    public partial class Artist : EntityBase
    {
        [Required]
        public string Name { get; set; }
        public bool IsSolo { get; set; }
        public ICollection<Song> Songs { get; set; }

    }
}