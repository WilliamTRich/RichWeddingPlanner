#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace RichWeddingPlanner.Models;
public class MyViewModel
{
    public User User { get; set; }
    public List<User> Users { get; set; }

    public Wedding Wedding { get; set; }
    public List<Wedding> Weddings { get; set; }

    public RSVP RSVP { get; set; }
    public List<RSVP> RSVPs { get; set; }
}