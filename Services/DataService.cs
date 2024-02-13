using FitnessPro.Data;
using FitnessPro.Enums;
using FitnessPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessPro.Services
{
    public class DataService
    {
       private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<FitnessUser> _userManager;
        public DataService(ApplicationDbContext context,
                           RoleManager<IdentityRole> roleManager,
                           UserManager<FitnessUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            //create the databse from migrations
            await _context.Database.MigrateAsync();

            //1. Seed roles into the system  
            await SeedRolesAsync();

            //2. Seed users into the system 
            await SeedUserAsync();
        }

        private async Task SeedRolesAsync()
        {
           
            //if there are already roles in the system do nothing 
            if (_context.Roles.Any())
            {
                return;
            }

            //Otherwise we want to create a few roles
            foreach (var role in Enum.GetNames(typeof(FitnessProRole)))
            {
                // I need to use the role manager to create roles 
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUserAsync()
        {
            //if there are already users in the system do nothing
            if (_context.Users.Any())
            {
                return;
            }
            // Step 1. Creates a new instance of FitnessUser
            var adminUser = new FitnessUser()
            {
                Email = "Khedinhoglaucio@gmail.com",
                UserName = "Khedinhoglaucio@gmail.com",
                FirstName = "Glaucio",
                LastName = "Alexandre",
                EmailConfirmed = true
            };
            //step 2: Use the UserManagger to create a new user that is defined by admiUser 
            await _userManager.CreateAsync(adminUser, "Abc&123!");
            //step 3: Add this new to the Admin role 
            await _userManager.AddToRoleAsync(adminUser, FitnessProRole.Admin.ToString());

            //Step 1 repeat: Create the trainer user 
            var trainerUser = new FitnessUser()
            {
                Email = "Davidfranco@gmail.com",
                UserName = "Davidfranco@gmail.com",
                FirstName = "David",
                LastName = "Franco",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(trainerUser, "Abc&123!");
            await _userManager.AddToRoleAsync(trainerUser, FitnessProRole.trainer.ToString());

            //Step 1 repeat: Create the trainer user 
            var clientUser = new FitnessUser()
            {
                Email = "Ola Bello",
                UserName = "olaBello@gmail.com",
                FirstName = "Ola",
                LastName = "Bello",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(clientUser, "Abc&123!");
            await _userManager.AddToRoleAsync(clientUser, FitnessProRole.client.ToString());
        }

    }
}
