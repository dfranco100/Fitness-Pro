using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FitnessPro.Models;

namespace FitnessPro.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        [Required]
        public string? FitnessUserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at leat {2} and a max {1} characters long", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at leat {2} and a max {1} characters long", MinimumLength = 2)]
        public string? LastName { get; set; }

        [Required]
        public string? Certification { get; set; }

        [Required]
        [Display(Name = "Years of Experience")]
        public int? Experience { get; set; }      
       
        [NotMapped]
        public string? FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        //virtual classes
        public virtual FitnessUser? FitnessUser { get; set; }

        public virtual ICollection<FitnessClass>? FitnessClasses { get; set; } = new HashSet<FitnessClass>();

    }
}
