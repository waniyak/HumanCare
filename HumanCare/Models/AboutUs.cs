using System.ComponentModel.DataAnnotations;

namespace HumanCare.Models
{
    public class AboutUs
    {
        [Key]
        public int Id { get; set; }
        public required string Content { get; set; }
    }

}
