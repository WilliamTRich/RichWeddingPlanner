using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RichWeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace RichWeddingPlanner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("users/create")]
    public IActionResult CreateUser(User newUser)
    {
        if (ModelState.IsValid)
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            return RedirectToAction("Weddings");
        }
        else
        {
            return View("Index");
        }
    }

    [HttpPost("users/login")]
    public IActionResult LoginUser(LogUser loginUser)
    {
        if (ModelState.IsValid)
        {
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == loginUser.LEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LEmail", "Invalid Email/Password");
                return View("Index");
            }
            PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
            var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LEmail", "Invalid Email/Password");
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Weddings");
            }
        }
        else
        {
            return View("Index");
        }
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [SessionCheck]
    [HttpGet("weddings/new")]
    public IActionResult NewWedding()
    {
        ViewBag.userId = HttpContext.Session.GetInt32("UserId");
        return View("PlanWedding");
    }

    [SessionCheck]
    [HttpGet("weddings")]
    public IActionResult Weddings()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        MyViewModel viewModel = new MyViewModel
        {
            Weddings = _context.Weddings.Include(w => w.RSVPs).ToList(),
            User = _context.Users.FirstOrDefault(u => u.UserId == userId),
        };
        return View("Weddings", viewModel);
    }

    [HttpGet("weddings/{id}/delete")]
    public IActionResult DeleteWedding(int id)
    {
        Wedding? delWed = _context.Weddings.FirstOrDefault(u => u.WeddingId == id);
        _context.Remove(delWed);
        _context.SaveChanges();
        return RedirectToAction("Weddings");
    }

    [HttpPost("weddings/{id}/rsvp")]
    public IActionResult RSVPWedding(MyViewModel viewModel)
    {
        RSVP? newRSVP = new RSVP
        {
            UserId = viewModel.User.UserId,
            WeddingId = viewModel.Wedding.WeddingId
        };
        _context.Add(newRSVP);
        _context.SaveChanges();
        return RedirectToAction("Weddings");
    }

    [HttpPost("weddings/{id}/unrsvp")]
    public IActionResult UnRSVPWedding(MyViewModel viewModel)
    {
        RSVP? delRSVP = _context.RSVPs.Where(r => r.User == viewModel.User).Where(r => r.Wedding == viewModel.Wedding).FirstOrDefault();
        _context.Remove(delRSVP);
        _context.SaveChanges();
        return RedirectToAction("Weddings");
    }

    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if (ModelState.IsValid)
        {
            _context.Add(newWedding);
            _context.SaveChanges();
            RSVP? newRSVP = new RSVP
            {
                UserId = newWedding.UserId,
                WeddingId = newWedding.WeddingId
            };
            _context.Add(newRSVP);
            _context.SaveChanges();
            return RedirectToAction("SingleWedding", new { id = newWedding.WeddingId });
        }
        else return View("PlanWedding");
    }

    [HttpGet("weddings/{id}")]
    public IActionResult SingleWedding(int id)
    {
        Wedding? SW = _context.Weddings.Include(w => w.RSVPs).ThenInclude(w => w.User).FirstOrDefault(w => w.WeddingId == id);
        MyViewModel viewModel = new MyViewModel
        {
            Wedding = SW,
            RSVPs = SW.RSVPs,
            Users = _context.Users.ToList()
        };
        return View("SingleWedding", viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}