namespace HumanCare.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public DateTime DonationDate { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int NgoId { get; set; } // New property for NGO
        public DonationCategory Category { get; set; }
        public Ngo Ngo { get; set; } // Navigation property
    }

}
