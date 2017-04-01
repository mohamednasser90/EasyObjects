using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
    public class Region
    {
        [Key]
        public int RegionID { get; set; }

        [Required]
        [StringLength(50)]
        public string RegionDescription { get; set; }
    }
}
