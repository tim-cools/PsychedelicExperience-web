using System;
using System.Linq;
using PsychedelicExperience.Common;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    internal static class ResponseExtensions
    {
        internal static Guid NewCreatedId(this IRestResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var location = GetLocation(response);

            var parts = location.Split('/');

            var id = GetGuid(parts[parts.Length - 1]);
            if (!id.HasValue)
            {
                throw new InvalidOperationException("Location header is not a GUID: " + location);
            }
            return id.Value;
        }

        private static Guid? GetGuid(string value)
        {
            if (value.Length == ShortGuid.Empty.Value.Length)
            {
                return new ShortGuid(value);
            }
            Guid guid;
            if (!Guid.TryParse(value, out guid))
            {
                throw new InvalidOperationException("Could not serialize the Id");
            }
            return guid;
        }


        private static string GetLocation(IRestResponse response)
        {
            var locationParameter = response.Headers.FirstOrDefault(
                    header => header.Key.Equals("location", StringComparison.OrdinalIgnoreCase));

            var location = locationParameter.Value.FirstOrDefault();
            if (location == null)
            {
                throw new InvalidOperationException("Location header is not set");
            }
            return location;
        }
    }
}