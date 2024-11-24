using HumanCare.Data;
using HumanCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using HumanCare.Models.ViewModels;
using System.Diagnostics;

namespace HumanCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //AboutUs
        // GET: Home/AboutUs
        public async Task<IActionResult> AboutUs()
        {
            var aboutUs = await _context.Aboutus.FirstOrDefaultAsync();
            if (aboutUs == null)
            {
                return NotFound();
            }
            return View(aboutUs);
        }

        public IActionResult Gallary()
        {
            return View();
        }


        //OurPartners
        public async Task<IActionResult> OurPartner()
        {
            var partners = await _context.Partners.ToListAsync();
            return View(partners); // Pass the list to the view
        }

        public async Task<IActionResult> AddDonation()
        {
            var viewModel = new DonationFormViewModel
            {
                Donation = new Donation(),
                Categories = await _context.DonationCategories.ToListAsync(),
                Ngos = await _context.Ngos.ToListAsync()
            };

            return View(viewModel);
        }

        //[Authorize]
        // POST: Donations/AddDonation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDonation(DonationFormViewModel viewModel)
        {
            // Set the current date as DonationDate
            viewModel.Donation.DonationDate = DateTime.Now;

            // Add the donation to the context
            _context.Add(viewModel.Donation);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the home page after successful addition
            return RedirectToAction("Index", "Home");
        }


        //ContactUs


        // GET: /User/ContactUs
        public IActionResult ContactUs()
        {
            return View();
        }

        // POST: /User/ContactUs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                // Add the contactUs object to the database
                _context.ContactUs.Add(contactUs);
                await _context.SaveChangesAsync();

                // Redirect or return a view as appropriate
                return RedirectToAction("ThankYou");
            }

            // If model state is not valid, return the same view with validation messages
            return View(contactUs);
        }

        // GET: /User/ThankYou
        public IActionResult ThankYou()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
