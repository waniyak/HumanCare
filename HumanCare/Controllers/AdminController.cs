using HumanCare.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanCare.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;

namespace HumanCare.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {               // Fetch counts from the new DbSets

            int userCount = await _dbContext.Users.CountAsync();
            int donationCount = await _dbContext.Donations.CountAsync();
            int partnerCount = await _dbContext.Partners.CountAsync();
            int ngoCount = await _dbContext.Ngos.CountAsync();

            // Pass the counts to the view
            ViewBag.UserCount = userCount;
            ViewBag.DonationCount = donationCount;
            ViewBag.PartnerCount = partnerCount;
            ViewBag.NgoCount = ngoCount;

            return View();
        }


        public async Task<IActionResult> ContactData()
        {
            // Fetch all contact messages from the database
            var contactMessages = await _dbContext.ContactUs.ToListAsync();

            // Pass the list to the view
            return View(contactMessages);
        }


        // manage donations,
        public async Task<IActionResult> DonationList()
        {
            var data = await _dbContext.Donations.Include(d => d.Category).Include(d => d.Ngo) 
                .ToListAsync();
            return View(data);
        }

        // add/edit partners,

        // update about us content,
        //

        //GET: AboutUs/EditAboutus
        public async Task<IActionResult> AboutUsData()
        {
            var data = await _dbContext.Aboutus.ToListAsync();
            return View(data);
        }

        // GET: AboutUs/EditAboutus
        [HttpGet]
        public async Task<IActionResult> EditAboutus()
        {
            var aboutUs = await _dbContext.Aboutus.FirstOrDefaultAsync(); // Ensure DbSet is named correctly
            if (aboutUs == null)
            {
                return NotFound();
            }
            return View(aboutUs);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAboutus(AboutUs aboutUs)
        {
            if (ModelState.IsValid)
            {
                // Fetch the entity from the database
                var existingAboutUs = await _dbContext.Aboutus
                                                      .FirstOrDefaultAsync(a => a.Id == aboutUs.Id);
                if (existingAboutUs == null)
                {
                    return NotFound();
                }

                // Update the properties
                existingAboutUs.Content = aboutUs.Content;

                // No need to call Update() method as existingAboutUs is already being tracked
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("AboutUsData");
            }

            return View(aboutUs);
        }




        // view members,
        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            return View(users);


        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }



            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(ListUsers)); // Redirect to the list of users or any other appropriate action
        }
    }
}
