using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanCare.Data;
using HumanCare.Models.ViewModels;
using HumanCare.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;


//[Authorize(Roles = "Admin")]
public class PartnersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public PartnersController(ApplicationDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    // This method runs before every action in this controller
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userRole = _httpContextAccessor.HttpContext.Session.GetString("UserRole");
        var isLoggedInString = _httpContextAccessor.HttpContext.Session.GetString("IsLoggedIn");
        if (isLoggedInString != "true" || userRole != "Admin")
        {
            context.Result = RedirectToAction("Login", "Account");
            return; // Skip further execution
        }

        base.OnActionExecuting(context);
    }



    // GET: Partners
    public async Task<IActionResult> OurPartners()
    {
        return View(await _context.Partners.ToListAsync());
    }

    // GET: Partners/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var partner = await _context.Partners
            .FirstOrDefaultAsync(m => m.Id == id);
        if (partner == null)
        {
            return NotFound();
        }

        return View(partner);
    }

    // GET: Partner/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartnerViewModel model)
    {
        // Handle the file upload
        string uniqueFileName = "images/noimage.PNG"; // Default image if no photo is uploaded

        if (model.Photo != null && model.Photo.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the uploaded file to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Photo.CopyToAsync(fileStream);
            }
        }

        // Create a new Partner object
        Partner newPartner = new Partner
        {
            Name = model.Name,
            Description = model.Description,
            Photo = uniqueFileName // Store the path of the uploaded photo
        };

        // Add the new partner to the database
        _context.Partners.Add(newPartner);
        await _context.SaveChangesAsync();

        // Redirect to the OurPartners action after successful creation
        return RedirectToAction("OurPartners");
    }




    // GET: Partners/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var partner = await _context.Partners.FindAsync(id);
        if (partner == null)
        {
            return NotFound(); // Return 404 if partner not found
        }

        var model = new PartnerViewModel
        {
            Id = partner.Id,
            Name = partner.Name,
            Description = partner.Description,
            ExistingPhotoPath = partner.Photo // Set the existing photo path
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PartnerViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound(); // Return 404 if IDs do not match
        }

        // Find the existing partner
        var partnerToUpdate = await _context.Partners.FindAsync(id);
        if (partnerToUpdate == null)
        {
            return NotFound(); // Return 404 if partner not found
        }

        // Handle the file upload
        string uniqueFileName = partnerToUpdate.Photo; // Keep the existing photo by default

        if (model.Photo != null && model.Photo.Length > 0)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Ensure the uploads folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Save the uploaded file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Photo.CopyToAsync(fileStream);
            }

            // Delete the old photo if it's not the default one
            var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", partnerToUpdate.Photo);
            if (System.IO.File.Exists(oldPhotoPath) && partnerToUpdate.Photo != "images/noimage.PNG")
            {
                System.IO.File.Delete(oldPhotoPath);
            }
        }

        // Update the partner's properties
        partnerToUpdate.Name = model.Name;
        partnerToUpdate.Description = model.Description;
        partnerToUpdate.Photo = uniqueFileName; // Update photo path

        // Save changes to the database
        _context.Update(partnerToUpdate);
        await _context.SaveChangesAsync();

        return RedirectToAction("OurPartners");
    }


    // GET: Partners/Delete/5
    // GET: Partners/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var partner = await _context.Partners
            .FirstOrDefaultAsync(m => m.Id == id);
        if (partner == null)
        {
            return NotFound(); // Return 404 if partner not found
        }

        return View(partner);
    }


    // POST: Partners/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var partner = await _context.Partners.FindAsync(id);
        if (partner == null)
        {
            return NotFound(); // Handle case where partner is not found
        }

        // Delete the associated image if necessary
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", partner.Photo);
        if (System.IO.File.Exists(imagePath) && partner.Photo != "images/noimage.PNG")
        {
            System.IO.File.Delete(imagePath);
        }

        _context.Partners.Remove(partner);
        await _context.SaveChangesAsync();

        return RedirectToAction("OurPartners");
    }

}
