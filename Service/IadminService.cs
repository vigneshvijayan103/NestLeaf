using Microsoft.AspNetCore.Mvc;

namespace NestLeaf.Service
{
    public interface IadminService
    {
        Task<decimal> GetTotalRevenue();
        Task<decimal> GetRevenueByDateAsync(DateTime startDate, DateTime endDate);

        Task<int> TotalUsers();




    }
}
