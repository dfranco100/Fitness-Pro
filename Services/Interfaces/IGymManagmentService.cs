
using FitnessPro.Models;

namespace FitnessPro.Services.Interfaces
{
    public interface IGymManagmentService
    {
        Task AddFitnessClassToTrainerAsync(int trainerId, int fitnessClassId);
        Task<bool> isFitnessClassInTrainer(int trainerId, int fitnessClassId);

        Task<IEnumerable<Trainer>> GetUserTrainersAsync(string userId);
        Task<ICollection<int>> GetFitnessClassTrainerIdsAsync(int fitnessClassId);
        Task<ICollection<FitnessClass>> GetFitnessClassesTrainersAsync(int fitnessClassId);
        Task RemoveFitnessClassTrainerAsync(int trainerId, int fitnessClassId);

        //Clients
        Task<IEnumerable<Client>> GetUserClientsAsync(string userId);

        IEnumerable<Trainer> SearchForFitnessClasses(string searchString, string userId);
        


    }
}