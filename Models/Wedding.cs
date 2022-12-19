#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace RichWeddingPlanner.Models;
public class Wedding
{
    [Key]
    public int WeddingId { get; set; }
    [Required(ErrorMessage = "Wedder is required.")]
    public string WedOne { get; set; }
    [Required(ErrorMessage = "Wedder is required.")]
    public string WedTwo { get; set; }
    [Required(ErrorMessage = "Date is required.")]
    [DateGreaterThanToday]
    public DateTime Date { get; set; }
    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

    //One user, who created
    //List of RSVPS
    public int UserId { get; set; }
    public List<RSVP> RSVPs { get; set; } = new List<RSVP>();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class DateGreaterThanToday : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return "Date value should be a future date";
    }

    protected override ValidationResult IsValid(object objValue,
                                                   ValidationContext validationContext)
    {
        var dateValue = objValue as DateTime? ?? new DateTime();

        if (dateValue.Date < DateTime.Now.Date)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        return ValidationResult.Success;
    }
}
