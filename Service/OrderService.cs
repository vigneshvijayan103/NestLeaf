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
            // Validate address
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == dto.ShippingAddressId && a.UserId == userId && a.DeletedAt == null);

            if (address == null)
                return false;

            // Load product data
            var productIds = dto.Items.Select(x => x.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if (products.Count != productIds.Count)
                return false;

            // Check stock availability
            foreach (var item in dto.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return false;

                if (product.Quantity < item.Quantity)
                    return false; // Insufficient stock
            }

            var order = new Order
            {
                UserId = userId,
                PaymentMethod = dto.PaymentMethod,
                ShippingAddressId = dto.ShippingAddressId,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = products[item.ProductId];
                var price = product.Price;
                total += price * item.Quantity;

                // Decrease stock
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




    }
}
