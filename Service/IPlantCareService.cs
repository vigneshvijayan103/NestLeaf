using NestLeaf.Dto;

namespace NestLeaf.Service
{
    public interface IPlantCareService
    {
        Task<bool> AddIssueAsync(AssignIssueDto dto, int userId);
        Task<IEnumerable<PlantIssueViewDto>> GetUserIssuesAsync(int userId);
        Task<IEnumerable<PlantIssueViewDto>> GetAllIssuesAsync();
        Task<bool> ResolveIssueAsync(ResolveIssueDto dto);
    }
}

