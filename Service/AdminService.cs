using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NestLeaf.Enum;
using NestLeaf.Models;
using System.Data;

namespace NestLeaf.Service
{
    public class AdminService : IadminService
    {
        private readonly IDbConnection _connection;
        private readonly NestLeafDbContext _context;

        public AdminService(IDbConnection connection, NestLeafDbContext context)
        {
            _connection = connection;
            _context = context;
        }

        public async Task<decimal> GetTotalRevenue()
        {
            return  await _context.Orders
         .Where(o => o.Status == OrderStatus.Delivered)
         .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        }

        public async Task<decimal> GetRevenueByDateAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date; 
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            return await _context.Orders
        .Where(o => o.Status == OrderStatus.Delivered &&
                    o.OrderDate >= start && o.OrderDate <= end)
        .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        }
        public async Task<int> GetTotalProductsPurchasedAsync()
        {
            return await _context.OrderItems
                .Where(oi => oi.Order.Status == OrderStatus.Delivered)
                .SumAsync(oi => (int?)oi.Quantity) ?? 0;
        }

        public async Task<int> TotalUsers()
        {
            return await _context.Users
           .Where(u => u.Role == "user")
             .CountAsync();

        }



    }
}

