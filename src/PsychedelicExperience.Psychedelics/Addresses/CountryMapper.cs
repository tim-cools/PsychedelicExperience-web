using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Addresses
{
    public interface ICountryMapper
    {
        string GetCode(string country);
        string GetCountry(string code);
        IEnumerable<string> GetCountries();
    }

    public class CountryMapper : ICountryMapper
    {
        private readonly Dictionary<string, string> _countriesByCode;
        private readonly Dictionary<string, string> _countries;

        public CountryMapper()
        {
            _countriesByCode = typeof(CountryMapper).ReadResourceDictionary("Countries")
                .ToDictionary(pair => pair.Value.NormalizeForSearch(), pair => pair.Key);
            _countries = typeof(CountryMapper).ReadResourceDictionary("Countries")
                .ToDictionary(pair => pair.Key.NormalizeForSearch(), pair => pair.Value);
        }

        public string GetCountry(string code)
        {
            if (code == null) return null;

            string country;
            if (!_countriesByCode.TryGetValue(code.NormalizeForSearch(), out country))
            {
                throw new InvalidOperationException("Country by code not found: " + code);
            }
            return country;
        }

        public string GetCode(string country)
        {
            if (country == null) return null;
            if (IsCode(country))
            {
                return country;
            }

            string code;
            if (!_countries.TryGetValue(country.NormalizeForSearch(), out code))
            {
                throw new InvalidOperationException("Country not found: " + country);
            }
            return code;
        }

        public IEnumerable<string> GetCountries()
        {
            return _countries.Keys;
        }

        private bool IsCode(string country)
        {
            return country.Length == 2;
        }
    }
}