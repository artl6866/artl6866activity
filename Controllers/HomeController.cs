using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using belt.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace belt.Controllers
{
    public class HomeController : Controller
    {
        private ActivityContext dbContext;
        public HomeController(ActivityContext context)
        {
            dbContext = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("/register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Index");
                }
            
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User user = new User {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = newUser.Email,
                    Password = Hasher.HashPassword(newUser, newUser.Password)
                };
                dbContext.Add(user);
                dbContext.SaveChanges();

                // reach into db context and find the new users id that just registered
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                int? UserId = HttpContext.Session.GetInt32("UserId");

                return RedirectToAction("Dashboard");
            }
            else
            {
                System.Console.WriteLine("=========================");
                System.Console.WriteLine("Validation failed Register");
                System.Console.WriteLine("=========================");
                return View("Index");
            }
            
            
        }
        [HttpPost]
        [Route("/login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var userindb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginEmail);
                if(userindb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email and/or Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, userindb.Password, user.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email and/or Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userindb.UserId);
                int ? UserId = HttpContext.Session.GetInt32("UserId");
                System.Console.WriteLine(UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                System.Console.WriteLine("=========================");
                System.Console.WriteLine("Validation failed Login");
                System.Console.WriteLine("=========================");
                return View("Index");
            }
            
        }
        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("/dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            User CurrentUser =dbContext.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            List<Activity> Allactivities = dbContext.Activities
                                        .Include(a => a.Participants)
                                        .ThenInclude(b => b.Users).ToList();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Allactivities = Allactivities;
            return View ("Dashboard");

        }
        [HttpGet]
        [Route("/addActivity")]
        public IActionResult AddAcvity(Activity activity)
        {
            if(HttpContext.Session.GetInt32("UserId")==null)
            {
                return RedirectToAction("Index");
            }
            return View("AddActivity");
        }
        [HttpPost]
        [Route("/createActivity")]
        public IActionResult createActivity(Activity newActivity)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId is null)
            {
                RedirectToAction("Index");
            }
            
            if(ModelState.IsValid)
            {
                Activity activity = new Activity
                {
                    ActivityName = newActivity.ActivityName,
                    Description = newActivity.Description,
                    When = newActivity.When,
                    Duration = newActivity.Duration,
                    UserId = (int)UserId
                };
                dbContext.Add(activity);
                dbContext.SaveChanges();

                // HttpContext.Session.SetInt32("logged_in_userId", newWedding.UserId);
                return RedirectToAction("Dashboard");

            }
            return View("AddActivity");
        }
        [HttpGet]
        [Route("delete/{ActivityId}")]
        public IActionResult Delete(int ActivityId)
        {
            if(HttpContext.Session.GetInt32("UserId")==null)
            {
                return RedirectToAction("Index");
            }
            Activity delete = dbContext.Activities.FirstOrDefault(w => w.ActivityId == ActivityId);
            dbContext.Activities.Remove(delete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [Route("show/{ActivityId}")]
        public IActionResult Show(int ActivityId)
        {
            if(HttpContext.Session.GetInt32("UserId")== null)
            {
                return RedirectToAction("Index");
            }
            User CurrentUser = dbContext.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            Activity CurrentActivity = dbContext.Activities
            .Include(activity => activity.Participants)
            .ThenInclude(participant => participant.Users)
            .SingleOrDefault(activity => activity.ActivityId == ActivityId);
            ViewBag.CurrentActivity = CurrentActivity;
            return View(CurrentActivity);
        }
        [HttpGet]
        [Route("join/{ActivityId}")]
        public IActionResult join(int ActivityId)
        {
            if(HttpContext.Session.GetInt32("UserId")==null)
            {
                return RedirectToAction("Index");
            }
            User CurrentUser = dbContext.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            Activity CurrentActivity = dbContext.Activities
            .Include(activity => activity.Participants)
            .ThenInclude(participant => participant.Users)
            .SingleOrDefault(activity => activity.ActivityId ==ActivityId);
            Participant newparticipant = new Participant
            {
                UserId = CurrentUser.UserId,
                Users = CurrentUser,
                ActivityId = CurrentActivity.ActivityId,
                Activities = CurrentActivity
            };
            CurrentActivity.Participants.Add(newparticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [Route("leave/{ActivityId}")]
        public IActionResult leave(int ActivityId)
        {
            if(HttpContext.Session.GetInt32("UserId")==null)
            {
                return RedirectToAction("Index");
            }
            User CurrentUser = dbContext.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            Participant CurrentParticipant = dbContext.Participants.SingleOrDefault(participant => participant.UserId == HttpContext.Session.GetInt32("UserId")&& participant.ActivityId == ActivityId);
            Activity CurrentActivity = dbContext.Activities
            .Include(activity => activity.Participants)
            .ThenInclude(participant => participant.Users)
            .SingleOrDefault(activity => activity.ActivityId == ActivityId);
            CurrentActivity.Participants.Remove(CurrentParticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [Route("joinShow/{ActivityId}")]
        public IActionResult joinShow(int ActivityId)
        {
            if(HttpContext.Session.GetInt32("UserId")==null)
            {
                return RedirectToAction("Index");
            }
            User CurrentUser = dbContext.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            Activity CurrentActivity = dbContext.Activities
            .Include(activity => activity.Participants)
            .ThenInclude(participant => participant.Users)
            .SingleOrDefault(activity => activity.ActivityId ==ActivityId);
            Participant newparticipant = new Participant
            {
                UserId = CurrentUser.UserId,
                Users = CurrentUser,
                ActivityId = CurrentActivity.ActivityId,
                Activities = CurrentActivity
            };
            CurrentActivity.Participants.Add(newparticipant);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        
    }
    
    
}
