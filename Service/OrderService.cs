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
                PaymentStatus= "Pending",
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
            var orderDict = new Dictionary<int, OrderDto>();

            var result = await _connection.QueryAsync<OrderDto, OrderItemDto, OrderDto>(
                "GetOrders",
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Items = new List<OrderItemDto>();
                        orderDict.Add(currentOrder.Id, currentOrder);
                    }

                    if (item?.ProductId != 0)
                    {
                        currentOrder.Items.Add(item);
                    }

                    return currentOrder;
                },
                new { UserId = userId },
                commandType: CommandType.StoredProcedure,
                splitOn: "ProductId"
            );

            return orderDict.Values.ToList();

        }

        
        public async Task<OrderDto?> GetOrderById(int orderId, int userId)
        {
            var orderDict = new Dictionary<int, OrderDto>();

            var result = await _connection.QueryAsync<OrderDto, OrderItemDto, OrderDto>(
                "GetOrderById",
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Items = new List<OrderItemDto>();
                        orderDict.Add(currentOrder.Id, currentOrder);
                    }

                    if (item?.ProductId != 0)
                    {
                        currentOrder.Items.Add(item);
                    }

                    return currentOrder;
                },
                new { OrderId = orderId, UserId = userId },
                commandType: CommandType.StoredProcedure,
                splitOn: "ProductId"
            );

            return orderDict.Values.FirstOrDefault();
        }

        public async Task<bool> MakePayment(PaymentRequestDto dto, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", dto.OrderId);
            parameters.Add("@UserId", userId);
            parameters.Add("@PaymentMethod", dto.PaymentMethod);

            var result = await _connection.QueryFirstOrDefaultAsync<int>(
                 "AddPayment",
                parameters,
                 commandType: CommandType.StoredProcedure
             );

            return result == 1;
        }

        public async Task<bool> CancelOrder(int? userId, int orderId, string cancelledBy)
        {
            var result = await _connection.QuerySingleOrDefaultAsync<int>("CancelOrder",new { userId, orderId, cancelledBy }, commandType: CommandType.StoredProcedure);

            return result == 1;

        }


        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            var flatOrders = await _connection.QueryAsync<AdminOrderFlatDto>(
                "GetallOrders",
                commandType: CommandType.StoredProcedure
            );

            var orderDict = new Dictionary<int, AdminOrderDto>();

            foreach (var row in flatOrders)
            {
                if (!orderDict.ContainsKey(row.OrderId))
                {
                    orderDict[row.OrderId] = new AdminOrderDto
                    {
                        OrderId = row.OrderId,
                        Username = row.Username,
                        Email = row.Email,
                        OrderDate = row.OrderDate,
                        TotalAmount = row.TotalAmount,
                        PaymentStatus = row.PaymentStatus,
                        Status = row.Status,
                        Items = new List<OrderItemDto>()
                    };
                }

                var item = new OrderItemDto
                {
                    ProductId = row.ProductId,
                    ProductName = row.ProductName,
                    Quantity = row.Quantity,
                    Price = row.Price
                };

                orderDict[row.OrderId].Items.Add(item);
            }

            return orderDict.Values.ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int status)
        {
          
            var rowsAffected = await _connection.ExecuteAsync("UpdateOrderStatus", new { orderId, status }, commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
         
        }

      



    }
}
