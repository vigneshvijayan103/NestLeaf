using NestLeaf.Dto;
namespace NestLeaf.Service
{
    public interface IorderServices
    {
        Task<bool> AddOrderAsync(AddOrderDto dto, int userId);
        Task<List<OrderDto>> GetOrders(int userId);
        Task<OrderDto?> GetOrderById(int orderId, int userId);


    }
}
