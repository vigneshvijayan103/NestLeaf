using Dapper;
using Microsoft.Data.SqlClient;
using NestLeaf.Dto;
using NestLeaf.Models;
using System.Data;

namespace NestLeaf.Service
{
    public class PlantCareService:IPlantCareService
    {
        private readonly IDbConnection _connection;
       
        public PlantCareService(IDbConnection connection)
        {
            _connection = connection;
            
        }
        public async Task<bool> AddIssueAsync(AssignIssueDto dto, int userId)
        {
        
            var result = await _connection.ExecuteAsync("AddPlantIssue",
                new
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Title = dto.Title,
                    Description = dto.Description
                },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<IEnumerable<PlantIssueViewDto>> GetUserIssuesAsync(int userId)
        {
         
            var issues = await _connection.QueryAsync<PlantIssueViewDto>(
                "GetUserPlantIssues",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
            return issues;
        }

        public async Task<IEnumerable<PlantIssueViewDto>> GetAllIssuesAsync()
        {
           
            var issues = await _connection.QueryAsync<PlantIssueViewDto>(
                "GetAllPlantIssues",
                commandType: CommandType.StoredProcedure);
            return issues;
        }

        public async Task<bool> ResolveIssueAsync(ResolveIssueDto dto)
        {
         
            var result = await _connection.ExecuteAsync("ResolvePlantIssue",
                new
                {
                    IssueId = dto.IssueId,
                    Resolution = dto.Resolution
                },
                commandType: CommandType.StoredProcedure);
            return result > 0;
        }
    }
}
