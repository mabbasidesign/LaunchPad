using LaunchPad.DTO;
using LaunchPad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaunchPad.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[CREATE_ORDER] Validation failed. Errors: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return BadRequest(ModelState);
            }

            var result = await _orderService.CreateOrderAsync(request, cancellationToken);
            _logger.LogInformation("[CREATE_ORDER] Created order {OrderId} with {ItemCount} items.", result.Id, result.Items.Count);

            return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest("Order ID must be greater than 0.");
            }

            var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            var orders = await _orderService.GetOrdersAsync(pageNumber, pageSize, cancellationToken);
            return Ok(orders);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<OrderSummaryDto>> GetOrderSummary([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                return BadRequest("FromDate must be earlier than ToDate.");
            }

            var summary = await _orderService.GetOrderSummaryAsync(fromDate, toDate, cancellationToken);
            return Ok(summary);
        }

        [HttpGet("top-items")]
        public async Task<ActionResult<List<TopItemDto>>> GetTopItems([FromQuery] int limit = 5, CancellationToken cancellationToken = default)
        {
            if (limit <= 0 || limit > 100)
            {
                return BadRequest("Limit must be between 1 and 100.");
            }

            var items = await _orderService.GetTopItemsAsync(limit, cancellationToken);
            return Ok(items);
        }
    }
}
