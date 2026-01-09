using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace PsychedelicExperience.Common.Web
{
    public static class QueryString
    {
        public static string Build(NameValueCollection parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return null;

            var query = new StringBuilder();

            foreach (var key in parameters.AllKeys)
            {
                Build(parameters, key, query);
            }

            return query.ToString();
        }

        private static void Build(NameValueCollection parameters, string key, StringBuilder query)
        {
            var values = parameters.GetValues(key);
            if (values == null) return;

            foreach (var value in values)
            {
                query.Append(query.Length == 0 ? "?" : "&");
                query.Append(WebUtility.UrlEncode(key));
                query.Append("=");
                query.Append(WebUtility.UrlEncode(value));
            }
        }
    }
}