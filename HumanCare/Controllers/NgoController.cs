using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using HumanCare.Data;
using HumanCare.Models;
using System.Threading.Tasks;

public class NgoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NgoController(ApplicationDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    //This method runs before every action in this controller
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



    // GET: Ngo
    public async Task<IActionResult> NgoList()
    {
        var ngos = await _context.Ngos.ToListAsync();
        return View(ngos);
    }

    // GET: Ngo/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Ngo/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ngo ngo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(ngo);
            await _context.SaveChangesAsync();
            return RedirectToAction("NgoList");
        }
        return View(ngo);
    }

    // GET: Ngo/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var ngo = await _context.Ngos.FindAsync(id);
        if (ngo == null)
        {
            return NotFound();
        }
        return View(ngo);
    }

    // POST: Ngo/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Ngo ngo)
    {
        if (id != ngo.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(ngo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NgoExists(ngo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("NgoList");
        }
        return View(ngo);
    }

    // GET: Ngo/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var ngo = await _context.Ngos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ngo == null)
        {
            return NotFound();
        }

        return View(ngo);
    }

    // POST: Ngo/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ngo = await _context.Ngos.FindAsync(id);
        if (ngo != null)
        {
            _context.Ngos.Remove(ngo);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("NgoList");
    }

    private bool NgoExists(int id)
    {
        return _context.Ngos.Any(e => e.Id == id);
    }
}
