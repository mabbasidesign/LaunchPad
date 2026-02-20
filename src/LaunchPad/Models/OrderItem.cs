using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaunchPad.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100000.")]
        public int Quantity { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Unit price must be between 0.01 and 100000.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
    }
}
