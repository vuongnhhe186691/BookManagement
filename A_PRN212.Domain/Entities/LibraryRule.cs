using System.ComponentModel.DataAnnotations;

namespace A_PRN212.Domain.Entities
{
    public class LibraryRule
    {
        [Key]
        public int RuleId { get; set; }
        public string? Rule { get; set; }
    }
}
