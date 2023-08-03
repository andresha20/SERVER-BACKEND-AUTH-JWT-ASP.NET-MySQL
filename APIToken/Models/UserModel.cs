using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace APIToken.Models
{
    [Index(nameof(Document), nameof(User_email), nameof(Phone), IsUnique = true)]
    public class UserModel
    {
        [Key]
        [Required(ErrorMessage = "Required field")]
        public int User_id { get; set; }
        [Required(ErrorMessage = "Required field")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Must be at least 5 characters, max 30.")]
        public required string User_name { get; set; }
        [Required(ErrorMessage = "Required field")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Must be at least 5 characters, max 30.")]
        public required string User_lastname { get; set; }
        [Required(ErrorMessage = "Required field")]
        [Range(1, 3, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public required int Doc_type { get; set; }
        [Required(ErrorMessage = "Required field")]
        [Range(43000000, 1999999999, ErrorMessage = "Must be a valid Colombian ID number.")]
        public required int Document { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Range(1, 3, ErrorMessage = "Must be a value between 1 and 3.")]
        public required int Genre { get; set; }
        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
        public required string User_email { get; set;}
        [Required(ErrorMessage = "Required field")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Must be 10 characters.")]
        public required string Phone { get; set; }
        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{10,}$", ErrorMessage = "Password must at least be 10 characters of length, contain at least 1 special character, at least 1 number, uppercase and lowercase.")]
        public required string Password { get; set; }

        public string? Salt { get; set; }

        public string? Token { get; set; }


        public UserModel()
        {
        }
    }
}
