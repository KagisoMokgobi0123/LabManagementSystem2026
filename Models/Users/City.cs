namespace NMB_HLabSys.Models.Users
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }

        public int ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}
