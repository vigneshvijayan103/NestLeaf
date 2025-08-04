using Dapper;
using Microsoft.EntityFrameworkCore;
using NestLeaf.Dto;
using NestLeaf.Models;
using System.Data;
using System.Data.Common;

namespace NestLeaf.Service
{
    public class OrderService:IorderServices
    {
        private readonly IDbConnection _connection;
        private readonly NestLeafDbContext _context;
        public OrderService(IDbConnection connection, NestLeafDbContext context)
        {
            _connection = connection;
            _context = context;
        }

        public async Task<bool> AddOrderAsync(AddOrderDto dto, int userId)
        {
          
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == dto.ShippingAddressId && a.UserId == userId && a.DeletedAt == null);

            if (address == null)
                return false;

            
            var productIds = dto.Items.Select(x => x.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if (products.Count != productIds.Count)
                return false;

            
            foreach (var item in dto.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return false;

                if (product.Quantity < item.Quantity)
                    return false; 
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddressId = dto.ShippingAddressId,
                IsPaid= false,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = products[item.ProductId];
                var price = product.Price;
                total += price * item.Quantity;

                product.Quantity -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = price
                });
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<OrderDto>> GetOrders(int userId)
        {


          using  var result = await _connection.QueryMultipleAsync(
                "GetOrders",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            var orders = (await result.ReadAsync<OrderDto>()).ToList();
            var items = (await result.ReadAsync<OrderItemDto>()).ToList();

            foreach (var order in orders)
            {
                order.Items = items.Where(i => i.OrderId == order.Id).ToList();
                 
            }
            return orders;
        }




        public async Task<OrderDto?> GetOrderById(int orderId, int? userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", orderId);
            parameters.Add("@UserId", userId);

            using var result = await _connection.QueryMultipleAsync("GetOrderById", parameters, commandType: CommandType.StoredProcedure);

            var order = (await result.ReadAsync<OrderDto>()).FirstOrDefault();
            if (order == null)
                return null;

            var items = (await result.ReadAsync<OrderItemDto>()).ToList();

            order.Items = items;

            return order;

        }



        public async Task<bool> CancelOrder(int? userId, int orderId, string cancelledBy)
        {
            var result = await _connection.QuerySingleOrDefaultAsync<int>("CancelOrder",new { userId, orderId, cancelledBy }, commandType: CommandType.StoredProcedure);

            return result == 1;

        }


        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            using var result = await _connection.QueryMultipleAsync("GetAllOrders",commandType: CommandType.StoredProcedure);

            var orders = (await result.ReadAsync<AdminOrderDto>()).ToList();
            var items = (await result.ReadAsync<OrderItemDto>()).ToList();

            foreach (var order in orders)
            {
                order.Items = items.Where(i => i.OrderId == order.OrderId).ToList();

            }
            return orders;

        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int status)
        {
          
            var rowsAffected = await _connection.ExecuteAsync("UpdateOrderStatus", new { orderId, status }, commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
         
        }

      



    }
}
