using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FitnessPro.Models;

namespace FitnessPro.Models
{
    public class FitnessClass
    {
        public int Id { get; set; }
        //public int TrainerId { get; set; }
        //public int ClientId { get; set; }
       
        [Required]
        public string? FitnessUserId { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public string? Name { get; set; }

        [Required]
        public string? Schedule { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long", MinimumLength = 2)]
        public string? Description { get; set; }
        
        public byte[]? ImageData { get; set; }
        
        public string? ImageType { get; set; }

        //Virtual Propriety to create a foreign keys 
        public virtual FitnessUser? FitnessUser { get; set; }
        public virtual ICollection<Trainer> Trainers { get; set; }= new List<Trainer>();
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();    

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public int FitnessClassId { get; internal set; }
    }
}
