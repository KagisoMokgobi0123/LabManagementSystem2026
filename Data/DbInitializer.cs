using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Data
{
    public static class DbInitializer
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedProvinces(modelBuilder);
            SeedCities(modelBuilder);
        }


        private static void SeedProvinces(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<Province>().HasData(

                new Province
                {
                    ProvinceId = 1,
                    Name = "Eastern Cape"
                },

                new Province
                {
                    ProvinceId = 2,
                    Name = "Free State"
                },

                new Province
                {
                    ProvinceId = 3,
                    Name = "Gauteng"
                },

                new Province
                {
                    ProvinceId = 4,
                    Name = "KwaZulu-Natal"
                },

                new Province
                {
                    ProvinceId = 5,
                    Name = "Limpopo"
                },

                new Province
                {
                    ProvinceId = 6,
                    Name = "Mpumalanga"
                },

                new Province
                {
                    ProvinceId = 7,
                    Name = "North West"
                },

                new Province
                {
                    ProvinceId = 8,
                    Name = "Northern Cape"
                },

                new Province
                {
                    ProvinceId = 9,
                    Name = "Western Cape"
                }

            );
        }



        private static void SeedCities(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<City>().HasData(

                // Eastern Cape
                new City
                {
                    CityId = 1,
                    Name = "Gqeberha",
                    ProvinceId = 1
                },

                new City
                {
                    CityId = 2,
                    Name = "East London",
                    ProvinceId = 1
                },


                // Free State
                new City
                {
                    CityId = 3,
                    Name = "Bloemfontein",
                    ProvinceId = 2
                },


                // Gauteng
                new City
                {
                    CityId = 4,
                    Name = "Johannesburg",
                    ProvinceId = 3
                },

                new City
                {
                    CityId = 5,
                    Name = "Pretoria",
                    ProvinceId = 3
                },


                // KwaZulu-Natal
                new City
                {
                    CityId = 6,
                    Name = "Durban",
                    ProvinceId = 4
                },

                new City
                {
                    CityId = 7,
                    Name = "Pietermaritzburg",
                    ProvinceId = 4
                },


                // Limpopo
                new City
                {
                    CityId = 8,
                    Name = "Polokwane",
                    ProvinceId = 5
                },


                // Mpumalanga
                new City
                {
                    CityId = 9,
                    Name = "Mbombela",
                    ProvinceId = 6
                },


                // North West
                new City
                {
                    CityId = 10,
                    Name = "Mahikeng",
                    ProvinceId = 7
                },


                // Northern Cape
                new City
                {
                    CityId = 11,
                    Name = "Kimberley",
                    ProvinceId = 8
                },


                // Western Cape
                new City
                {
                    CityId = 12,
                    Name = "Cape Town",
                    ProvinceId = 9
                },

                new City
                {
                    CityId = 13,
                    Name = "George",
                    ProvinceId = 9
                }

            );
        }
    }
}