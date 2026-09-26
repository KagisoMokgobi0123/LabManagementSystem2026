using System.Collections.Generic;

namespace NMB_HLabSys.Models.Users
{
    public class Province
    {
        public int ProvinceId { get; set; }
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
