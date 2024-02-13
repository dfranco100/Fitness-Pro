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
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<FitnessUser> _userManager;
        public TrainersController(ApplicationDbContext context,
                                                                UserManager<FitnessUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trainers
        [Authorize]
        public Task<IActionResult> Index(int FitnessClassId)
        {
         
            var Trainers= new List<Trainer>();            
            string fitnessUserId = _userManager.GetUserId(User);

            //Return the userId and its associated Trainer and FitnessClasses
            FitnessUser fitnessUser = _context.Users.Include(c => c.Trainers)
                                                    .ThenInclude(c => c.FitnessClasses)
                                                    .FirstOrDefault(u => u.Id == fitnessUserId)!;

            var fitnessClasses = fitnessUser.FitnessClasses;
            if  (FitnessClassId == 0)
            {
                Trainers = fitnessUser.Trainers.OrderBy(c => c.LastName)
                                               .ThenBy(c => c.FirstName)
                                               .ToList();
            } else
            {
                Trainers = fitnessUser.FitnessClasses.FirstOrDefault(c => c.Id == FitnessClassId)!
                                                        .Trainers
                                                        .OrderBy(c => c.LastName)
                                                        .ThenBy(c => c.FirstName)
                                                        .ToList();
            }
            ViewData["FitnessClassId"] = new SelectList(fitnessClasses, "Id", "Name", FitnessClassId);


            return Task.FromResult<IActionResult>(View(Trainers));
        }

        //Action to handle the search filter
        [Authorize]
        public IActionResult SearchTrainers(string searchString)
        {
            string fitnessUserId = _userManager.GetUserId(User);
            var trainers = new List<Trainer>();

            FitnessUser fitnessUser = _context.Users
                                               .Include(c => c.Trainers)
                                               .ThenInclude(c => c.FitnessClasses)
                                               .FirstOrDefault(u => u.Id == fitnessUserId)!;

            if (String.IsNullOrEmpty(searchString))
            {
                trainers=fitnessUser.Trainers
                                       .OrderBy(c => c.LastName) 
                                       .ThenBy(c => c.FirstName)
                                       .ToList();
            }
            else
            {
                trainers = fitnessUser.Trainers
                                      .Where(c => c.FullName!.ToLower().Contains(searchString.ToLower()))
                                      .OrderBy(c => c.LastName)
                                      .ThenBy(c => c.FirstName)
                                      .ToList();
            }
            ViewData["FitnessClassId"] = new SelectList(fitnessUser.FitnessClasses, "Id", "Name", 0);

            return View(nameof(Index), trainers);
        }

        // GET: Trainers/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.FitnessUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id");

            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FitnessUserId,FirstName,LastName,Certification,Experience")] Trainer trainer)
        {
            //ModelState.Remove("FitnessUserId");

            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", trainer.FitnessUserId);
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", trainer.FitnessUserId);
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FitnessUserId,FirstName,LastName,Certification,Experience")] Trainer trainer)
        {
            if (id != trainer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id))
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
            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", trainer.FitnessUserId);
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Trainers == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.FitnessUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Trainers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Trainers'  is null.");
            }
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
          return (_context.Trainers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
