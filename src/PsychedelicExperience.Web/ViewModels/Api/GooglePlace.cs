using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PsychedelicExperience.Membership.Messages;

namespace PsychedelicExperience.Web.ViewModels.Api
{
    public class GooglePlace
    {
        private readonly IDictionary<string, IList<string>> _attributes = new Dictionary<string, IList<string>>();
        private readonly IList<GooglePlaceComponent> _components = new List<GooglePlaceComponent>();

        internal Address ToAddress()
        {
            var name = GetAttribute("formatted_address");
            var country = GetComponent("country");
            var location = GetLocation();
            var placeId = GetAttribute("place_id");

            if (name == null || location == null)
            {
                return null;
            }

            var attributes = new Dictionary<string, string>
            {
                {"origin", "google-place"},
                {"googlePlaceId", placeId}
            };

            return new Address(name, country, location, placeId, attributes);
        }

        private Location GetLocation()
        {
            var value = GetAttribute("geometry.location")?.Trim('(', ')');
            var parts = value?.Split(',');
            if (parts == null || parts.Length != 2) return null;

            decimal latitude;
            decimal longitude;

            if (!decimal.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out latitude)
                || !decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out longitude))
            {
                return null;
            }

            return new Location(latitude, longitude);
        }

        private string GetComponent(string name)
        {
            var component = _components.FirstOrDefault(criteria => criteria.Types.Contains(name));
            return component?.ShortName;
        }

        private string GetAttribute(string name)
        {
            IList<string> values;
            return !_attributes.TryGetValue(name, out values)
                ? null
                : string.Concat(values);
        }

        public void AddAttribute(string key, string value)
        {
            IList<string> list;
            if (!_attributes.TryGetValue(key, out list))
            {
                list = new List<string>();
                _attributes.Add(key, list);
            }
            list.Add(value);
        }

        public void AddComponent(GooglePlaceComponent component)
        {
            _components.Add(component);
        }
    }
}