using LaunchPad.DTO;

namespace LaunchPad.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderRequestDto request, CancellationToken cancellationToken);
        Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<OrderDto>> GetOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<OrderSummaryDto> GetOrderSummaryAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken);
        Task<List<TopItemDto>> GetTopItemsAsync(int limit, CancellationToken cancellationToken);
    }
}
