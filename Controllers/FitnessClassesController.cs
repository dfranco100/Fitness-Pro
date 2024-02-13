using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessPro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FitnessPro.Models;
using NuGet.DependencyResolver;
using FitnessPro.Services.Interfaces;
using FitnessPro.Services;

namespace FitnessPro.Controllers
{
    public class FitnessClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FitnessUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IGymManagmentService _managmentService;

        public FitnessClassesController(ApplicationDbContext context,
                                        UserManager<FitnessUser> userManager,
                                        IImageService imageService,
                                        IGymManagmentService managmentService) 

        {
            _context = context;
           _userManager = userManager;
            _imageService = imageService;
            _managmentService = managmentService;
        }

        // GET: FitnessClasses
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FitnessClasses.Include(f => f.FitnessUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FitnessClasses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FitnessClasses == null)
            {
                return NotFound();
            }

            var fitnessClass = await _context.FitnessClasses
                .Include(f => f.FitnessUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fitnessClass == null)
            {
                return NotFound();
            }

            return View(fitnessClass);
        }

        // GET: FitnessClasses/Create
        [Authorize]
       

        public async Task<IActionResult> Create()
        {
            string fitnessUserId = _userManager.GetUserId(User);

            var trainers = await _managmentService.GetUserTrainersAsync(fitnessUserId);
            var clients = await _managmentService.GetUserClientsAsync(fitnessUserId);

            // Create a SelectListItems array with FirstName as the text and Certification as an additional information for Trainers
            var trainerItems = trainers.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(), // Assuming Id is used as the value
                Text = $"{t.FirstName}=> {t.Certification}" // Display FirstName and Certification
            }).ToList();

            ViewData["TrainerList"] = new MultiSelectList(trainerItems, "Value", "Text");

            // Create a SelectListItems array with FirstName as the text and LastName as an additional info for Clients
            var clientItems = clients.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(), // Assuming Id is used as the value
                Text = $"{t.FirstName} {t.LastName}" // Display FirstName and Lastname
            }).ToList();

            ViewData["ClientList"] = new MultiSelectList(clientItems, "Value", "Text");

            return View();
        }



        // POST: FitnessClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrainerId,ClientId,Name,Schedule,Description,ImageFile")] FitnessClass fitnessClass, List<int>TrainerList)
        {
            //ModelState.Remove to make sure we do not have to select the user id manually and avoid data diplay errors.
            ModelState.Remove("FitnessUserId");
           
            if (ModelState.IsValid)
            {
                fitnessClass.FitnessUserId = _userManager.GetUserId(User);

                if (fitnessClass.ImageFile != null)
                {
                    fitnessClass.ImageData = await _imageService.ConvertFileToByteArrayAsync(fitnessClass.ImageFile);
                    fitnessClass.ImageType = fitnessClass.ImageFile.ContentType;
                }

                _context.Add(fitnessClass);
                await _context.SaveChangesAsync();

                //Loop over all the selected Trainers and save each Trainer to the joint table TrainerFitnessClass.
                foreach(int trainerId in TrainerList)
                {
                    await _managmentService.AddFitnessClassToTrainerAsync(trainerId, fitnessClass.Id);
                }

                return RedirectToAction(nameof(Index));
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: FitnessClasses/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null || _context.FitnessClasses == null)
            {
                return NotFound();
            }

            string fitnessUserId=  _userManager.GetUserId(User);


            //var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            var fitnessClass = await _context.FitnessClasses.Where(c => c.Id == id && c.FitnessUserId == fitnessUserId)
                                                             .FirstOrDefaultAsync();  
            if (fitnessClass == null)
            {
                return NotFound();
            }

            //Handle fitnessClass inside edit form 
            var trainers = await _managmentService.GetUserTrainersAsync(fitnessUserId);

            var trainerItems = trainers.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(), // Assuming Id is used as the value
                Text = $"{t.FirstName}=> {t.Certification}" // Display FirstName and Certification
            }).ToList();

            ViewData["TrainerList"] = new MultiSelectList(trainerItems, "Value", "Text", await _managmentService.GetFitnessClassTrainerIdsAsync(fitnessClass.Id));

            return View(fitnessClass);
        }

        // POST: FitnessClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrainerId,ClientId,FitnessUserId,Name,Schedule,Description,ImageData,ImageType, ImageFile")] FitnessClass fitnessClass, List<int> TrainerList)
        {
            if (id != fitnessClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(fitnessClass.ImageType!=null)
                    {
                        fitnessClass.ImageData = await _imageService.ConvertFileToByteArrayAsync(fitnessClass.ImageFile);
                        fitnessClass.ImageType = fitnessClass.ImageFile.ContentType;
                    }
                    _context.Update(fitnessClass);
                    await _context.SaveChangesAsync();

                    List<Trainer> oldTrainers= (List<Trainer>)await _managmentService.GetFitnessClassesTrainersAsync(fitnessClass.Id);
                    foreach (var trainer in oldTrainers)
                    {
                        await _managmentService.RemoveFitnessClassTrainerAsync(trainer.Id, fitnessClass.Id);
                    }
                    foreach(int trainerId in TrainerList)
                    {
                        await _managmentService.AddFitnessClassToTrainerAsync(trainerId, fitnessClass.Id);
                    }
                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessClassExists(fitnessClass.Id))
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

            ViewData["FitnessUserId"] = new SelectList(_context.Users, "Id", "Id", fitnessClass.FitnessUserId);
            return View(fitnessClass);
        }

        // GET: FitnessClasses/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FitnessClasses == null)
            {
                return NotFound();
            }

            var fitnessClass = await _context.FitnessClasses
                .Include(f => f.FitnessUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fitnessClass == null)
            {
                return NotFound();
            }

            return View(fitnessClass);
        }

        // POST: FitnessClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FitnessClasses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FitnessClasses'  is null.");
            }
            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass != null)
            {
                _context.FitnessClasses.Remove(fitnessClass);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FitnessClassExists(int id)
        {
          return (_context.FitnessClasses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
