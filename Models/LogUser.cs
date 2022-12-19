#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace RichWeddingPlanner.Models;
public class LogUser
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string LEmail { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be greater than 7 characters.")]
    [DataType(DataType.Password)]
    public string LPassword { get; set; }
}