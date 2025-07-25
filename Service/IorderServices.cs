using NestLeaf.Dto;
namespace NestLeaf.Service
{
    public interface IorderServices
    {
        Task<bool> AddOrderAsync(AddOrderDto dto, int userId);
        Task<List<OrderDto>> GetOrders(int userId);
        Task<OrderDto?> GetOrderById(int orderId, int userId);

        Task<bool> CancelOrder(int? userId, int orderId, string cancelledBy);

        Task<List<AdminOrderDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, int status);

        Task<bool> DeleteOrder(int orderId);


    }
}
