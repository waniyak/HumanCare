using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanCare.Models.ViewModels
{
    public class PartnerViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; } // Make this property nullable
        public string ExistingPhotoPath { get; set; } // For displaying existing photo
    }

}
