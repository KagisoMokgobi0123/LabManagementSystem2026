using NMB_HLabSys.Data;
using NMB_HLabSys.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;


        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<List<SelectListItem>> GetProvincesAsync()
        {
            return await _context.Provinces
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.ProvinceId.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }



        public async Task<List<SelectListItem>> GetCitiesAsync(
            int provinceId)
        {
            return await _context.Cities
                .Where(x => x.ProvinceId == provinceId)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.CityId.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }
    }
}