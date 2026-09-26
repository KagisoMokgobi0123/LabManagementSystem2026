using Microsoft.AspNetCore.Mvc.Rendering;

namespace NMB_HLabSys.Services.Interfaces
{
    public interface ILocationService
    {
        Task<List<SelectListItem>> GetProvincesAsync();

        Task<List<SelectListItem>> GetCitiesAsync(int provinceId);
    }
}
