using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Id")]
        public string? Username  { get; set; }
        
        [Required]
        public string? Password { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
