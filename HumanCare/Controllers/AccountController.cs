using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Security.Cryptography;
using HumanCare.Models;
using HumanCare.Models.ViewModels;
using HumanCare.Data;
using System.Data;
using System.Text.RegularExpressions;
using HumanCare.Data;

namespace HumanCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(ILogger<AccountController> logger, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (model.Email.Equals("admin@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "The email address 'admin@gmail.com' is not allowed.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "The password and confirmation password do not match.");
                return View(model);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new Users
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword
            };

            if (ModelState.IsValid)
            {
                // Ensure roles exist
                await EnsureRolesExist();

                // Assign user role
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                if (userRole != null)
                {
                    user.UserRoles = new List<UserRole>
            {
                new UserRole { User = user, Role = userRole }
            };
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User role not found.");
                    return View(model);
                }

                // Add user to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Store username in session (if needed for subsequent steps)
                HttpContext.Session.SetString("Username", model.Username);

                _logger.LogInformation("User created a new account.");

                // Redirect to login after successful registration
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }


        private async Task EnsureRolesExist()
        {
            if (!_context.Roles.Any(r => r.Name == "user"))
            {
                _context.Roles.Add(new Role { Name = "user" });
                _logger.LogInformation("user role created.");
            }

            await _context.SaveChangesAsync();
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToHexString(bytes);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var authenticatedUser = AuthenticateUser(model.Email, model.Password);

                if (authenticatedUser != null && BCrypt.Net.BCrypt.Verify(model.Password, authenticatedUser.PasswordHash))
                {
                    // Set session variables
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("Username", authenticatedUser.Username);

                    var userRole = authenticatedUser.UserRoles.FirstOrDefault()?.Role?.Name;
                    if (userRole != null)
                    {
                        HttpContext.Session.SetString("UserRole", userRole);
                        // On successful login, set a session key
                        HttpContext.Session.SetString("UserSession", "Active");
                    }

                    _logger.LogInformation("User logged in successfully.");

                    // Redirect based on user role
                    if (userRole == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (userRole == "user")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials. Please try again.");
                }
            }

            // If ModelState is not valid or authentication fails, return to login view with errors
            return View(model);
        }

        private Users AuthenticateUser(string email, string password)
        {
            // Replace with actual authentication logic (e.g., querying database)
            var userFromDb = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Email == email);

            if (userFromDb != null)
            {
                // Verify password using BCrypt
                if (BCrypt.Net.BCrypt.Verify(password, userFromDb.PasswordHash))
                {
                    return userFromDb; // Return user object on successful authentication
                }
            }

            return null; // Return null if authentication fails
        }
        
        
        [HttpGet]
        public IActionResult CheckSession()
        {
            bool isActive = HttpContext.Session.Keys.Any(); // Check if any keys exist in session
            return Json(new { isActive });
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear all session data
                                         //_logger.LogInformation("User logged out.");
            Response.Cookies.Delete(".AspNetCore.Session");

            return RedirectToAction("Login", "Account");
        }

    }
}
