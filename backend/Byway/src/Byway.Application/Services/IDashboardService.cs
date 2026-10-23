using Byway.Application.DTOs.Dashboard;

namespace Byway.Application.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(CancellationToken ct = default);
}
