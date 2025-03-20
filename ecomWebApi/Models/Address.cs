using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecomWebApi.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [MaxLength(10)]
        public string MobileNumber { get; set; }

        [Required]
        [MaxLength(6)]
        public string Pincode { get; set; }

        [Required]
        public string FlatHouseNo { get; set; }

        [Required]
        public string AreaStreet { get; set; }

        public string? Landmark { get; set; }  // Optional

        [Required]
        public string TownCity { get; set; }

        // Foreign key relationship with User
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
