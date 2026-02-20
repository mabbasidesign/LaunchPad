using LaunchPad.Data;
using LaunchPad.DTO;
using LaunchPad.Models;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Services
{
    public class OrderService : IOrderService
    {
        private const decimal DefaultTaxRate = 0.08m;
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequestDto request, CancellationToken cancellationToken)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("At least one order item is required.", nameof(request.Items));
            }

            var discountPercent = request.DiscountPercent;
            if (discountPercent < 0m || discountPercent > 1m)
            {
                throw new ArgumentOutOfRangeException(nameof(request.DiscountPercent), "Discount percent must be between 0 and 1.");
            }

            var orderItems = request.Items.Select(item => new OrderItem
            {
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = Math.Round(item.UnitPrice * item.Quantity, 2)
            }).ToList();

            var subtotal = orderItems.Sum(i => i.LineTotal);
            var discountAmount = Math.Round(subtotal * discountPercent, 2);
            var taxableAmount = subtotal - discountAmount;
            var taxAmount = Math.Round(taxableAmount * DefaultTaxRate, 2);
            var total = Math.Round(taxableAmount + taxAmount, 2);

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                Subtotal = subtotal,
                DiscountPercent = discountPercent,
                DiscountAmount = discountAmount,
                TaxRate = DefaultTaxRate,
                TaxAmount = taxAmount,
                Total = total,
                Items = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return MapOrderToDto(order);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            return order == null ? null : MapOrderToDto(order);
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= toDate.Value);
            }

            var summary = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.Total),
                    TotalTax = g.Sum(o => o.TaxAmount),
                    TotalDiscount = g.Sum(o => o.DiscountAmount)
                })
                .FirstOrDefaultAsync(cancellationToken);

            var totalOrders = summary?.TotalOrders ?? 0;
            var totalRevenue = summary?.TotalRevenue ?? 0m;
            var totalTax = summary?.TotalTax ?? 0m;
            var totalDiscount = summary?.TotalDiscount ?? 0m;

            return new OrderSummaryDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalTax = totalTax,
                TotalDiscount = totalDiscount,
                AverageOrderValue = totalOrders > 0 ? Math.Round(totalRevenue / totalOrders, 2) : 0m,
                FromDate = fromDate,
                ToDate = toDate
            };
        }

        public async Task<List<TopItemDto>> GetTopItemsAsync(int limit, CancellationToken cancellationToken)
        {
            var topItems = await _context.OrderItems
                .AsNoTracking()
                .GroupBy(i => i.ProductName)
                .Select(g => new TopItemDto
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.LineTotal)
                })
                .OrderByDescending(i => i.TotalRevenue)
                .ThenByDescending(i => i.TotalQuantity)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return topItems;
        }

        private static OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Subtotal = order.Subtotal,
                DiscountPercent = order.DiscountPercent,
                DiscountAmount = order.DiscountAmount,
                TaxRate = order.TaxRate,
                TaxAmount = order.TaxAmount,
                Total = order.Total,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            };
        }
    }
}
