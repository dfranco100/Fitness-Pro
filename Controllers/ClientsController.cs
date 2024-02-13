using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessPro.Data;
using FitnessPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FitnessPro.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FitnessUser> _userManager;

        public ClientsController(ApplicationDbContext context, UserManager<FitnessUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Clients
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clients.Include(c => c.FitnessUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clients/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.FitnessUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, FitnessUserId,FirstName,LastName,Age,FitnessGoal")] Client client)
        {
            //ModelState.Remove("FitnessUserId");

            if (ModelState.IsValid)
            {
                ModelState.Remove("FitnessUserId");

                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                client.FitnessUserId = currentUser.Id; // Set the FitnessUserId to the current user's ID

                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", client.FitnessUserId);
            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            //Allow only users with admin roles to access the edit action of any user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (!User.IsInRole("Admin"))
            {
                if (client.FitnessUserId != currentUser.Id)
                {
                    // Unauthorized access to edit other user's client details
                    return Forbid();
                }
            }


            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", client.FitnessUserId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FitnessUserId,FirstName,LastName,Age,FitnessGoal")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", client.FitnessUserId);
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.FitnessUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return (_context.Clients?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
