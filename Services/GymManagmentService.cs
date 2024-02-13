using FitnessPro.Data;
using FitnessPro.Models;
using FitnessPro.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessPro.Services
{
    public class GymManagmentService : IGymManagmentService
    {
        private readonly ApplicationDbContext _context;

        public GymManagmentService(ApplicationDbContext context)//Connect services to the database
        {
            _context = context;
        }


        public async Task AddFitnessClassToTrainerAsync( int trainerId, int fitnessClassId)
        {
            try
            {
                // check if the trainer is int the FitnessClass
                if(!await isFitnessClassInTrainer(trainerId, fitnessClassId))
                {
                    FitnessClass? fitnessClass = await _context.FitnessClasses.FindAsync(fitnessClassId);
                    Trainer? trainer = await _context.Trainers.FindAsync(trainerId);
                    if (trainer != null && fitnessClass!=null) 
                    {
                        trainer.FitnessClasses!.Add(fitnessClass);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<ICollection<FitnessClass>> GetFitnessClassesTrainersAsync(int fitnessClassId)
        {
            try
            {
                FitnessClass? fitnessClass = await _context.FitnessClasses.Include(c => c.Trainers).FirstOrDefaultAsync(c => c.Id == fitnessClassId);
                return (ICollection<FitnessClass>)fitnessClass!.Trainers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<int>> GetFitnessClassTrainerIdsAsync(int fitnessClassId)
        {
            try
            {
                var fitnessClass=await _context.FitnessClasses.Include(c => c.Trainers)
                                                    .FirstOrDefaultAsync(c => c.Id == fitnessClassId);

                List<int> trainersIds = fitnessClass!.Trainers.Select(c => c.Id).ToList();
                return trainersIds;

            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<IEnumerable<Trainer>> GetUserTrainersAsync(string userId)
        {
            List<Trainer> trainers = new List<Trainer>();
            try
            {
                trainers= await _context.Trainers.Where(c => c.FitnessUserId == userId)
                                                                                .OrderBy(c => c.FirstName)                                                                             
                                                                                 .ToListAsync();
            }
            catch
            {
                throw;
            }

            return trainers;
        }

        //handle Clients
        public async Task<IEnumerable<Client>> GetUserClientsAsync(string userId)
        {
            List<Client> clients = new List<Client>();
            try
            {
                clients = await _context.Clients.Where(c => c.FitnessUserId == userId)
                                                                                .OrderBy(c => c.FirstName)
                                                                                 .ToListAsync();
            }
            catch
            {
                throw;
            }

            return clients;

        }

        public async Task<bool> isFitnessClassInTrainer(int trainerId, int fitnessClassId )
        {
            FitnessClass? fitnessClass = await _context.FitnessClasses.FindAsync(fitnessClassId);
            return await _context.Trainers
                                  .Include(c => c.FitnessClasses)
                                  .Where(c => c.Id == trainerId && c.FitnessClasses!.Contains(fitnessClass!))
                                  .AnyAsync();
        }

        public async Task RemoveFitnessClassTrainerAsync( int trainerId, int fitnessClassId)
        {
            try
            {
                if (await isFitnessClassInTrainer(trainerId, fitnessClassId))
                {
                    FitnessClass fitnessClass = await _context.FitnessClasses.FindAsync(fitnessClassId);
                    Trainer trainer= await _context.Trainers.FindAsync(trainerId);
                    if(trainer != null && fitnessClass!=null)
                    {
                        trainer.FitnessClasses!.Remove(fitnessClass);
                        await _context.SaveChangesAsync();
                    }
                }
            }catch
            {
                throw;
            }
        }

        public IEnumerable<Trainer> SearchForFitnessClasses(string searchString, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
