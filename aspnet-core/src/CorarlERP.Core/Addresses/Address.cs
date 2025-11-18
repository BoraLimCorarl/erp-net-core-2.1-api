using Abp.Domain.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Addresses
{
   
    public class Address: ValueObject<Address>
    {
        public const int MaxCountryLength = 3;
        public const int MaxStreetLength = 512;
        public const int MaxCityTownLength = 256;
        public const int MaxProvinceLength = 256;
        public const int MaxPostalCodeLength = 64;

        public Address (string cityTown, string country, string postalCode, string province, string street)
        {

            CityTown = cityTown;
            Country = country;
            PostalCode = postalCode;
            Province = province;
            Street = street;
        }
        internal Address() { }

        [MaxLength(MaxStreetLength)]
        public string Street { get;private  set; }

        [MaxLength(MaxCityTownLength)]
        public string CityTown { get; private set; }

        [MaxLength(MaxProvinceLength)]
        public string Province { get;private set; }

        [MaxLength(MaxPostalCodeLength)]
        public string PostalCode { get;private set; }

        [MaxLength(MaxCountryLength)]
        public string Country { get; private set; }

        public void Update(Address address)
        {
            CityTown = address.CityTown;
            Street = address.Street;
            Province = address.Province;
            Country = address.Country;
            PostalCode = address.PostalCode;
        }
    }
}
