using System.ComponentModel.DataAnnotations;

namespace HumanCare.Models.ViewModels
{
    public class DonationFormViewModel
    {
        
        public Donation Donation { get; set; } // The donation being created or edited
        public IEnumerable<DonationCategory> Categories { get; set; } // List of categories for the dropdown
        public IEnumerable<Ngo> Ngos { get; set; } // List of NGOs for the dropdown
    }

}
