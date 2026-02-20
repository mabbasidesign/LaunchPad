using System.ComponentModel.DataAnnotations;

namespace LaunchPad.DTO
{
    public class CreateOrderItemRequestDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100000.")]
        public int Quantity { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Unit price must be between 0.01 and 100000.")]
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderRequestDto
    {
        [Range(0, 1, ErrorMessage = "Discount percent must be between 0 and 1.")]
        public decimal DiscountPercent { get; set; } = 0m;

        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        [Required(ErrorMessage = "Order items are required.")]
        public List<CreateOrderItemRequestDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class TopItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
