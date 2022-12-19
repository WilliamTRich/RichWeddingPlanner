#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RichWeddingPlanner.Models;
public class User
{
    [Key]
    public int UserId { get; set; }
    [Required(ErrorMessage = "First Name is required.")]
    [MinLength(2, ErrorMessage = "First name must be greater than two characters.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Last Name is required.")]
    [MinLength(2, ErrorMessage = "Last Name must be greater than two characters.")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [UniqueEmail]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be greater than 7 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    //List of Weddings
    //List of RSVPS
    public List<Wedding> Weddings { get; set; } = new List<Wedding>();
    public List<RSVP> RSVPs { get; set; } = new List<RSVP>();

    [NotMapped]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }
}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Email is required!");
        }

        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        if (_context.Users.Any(u => u.Email == value.ToString()))
        {
            return new ValidationResult("Email must be unique.");
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}