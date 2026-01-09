using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace PsychedelicExperience.Web.Controllers
{
    public class ResultBuilder<T>
    {
        private bool _notFoundWhenNull;
        private Func<T, string> _urlSelector;
        private Func<T, bool> _redirectCriteria;
        private QueryString _queryString;

        public ResultBuilder<T> NotFoundWhenNull()
        {
            _notFoundWhenNull = true;
            return this;
        }

        public ResultBuilder<T> Redirect(Func<T, string> urlSelector)
        {
            _urlSelector = urlSelector ?? throw new ArgumentNullException(nameof(urlSelector));
            _redirectCriteria = value => true;

            return this;
        }

        public ResultBuilder<T> RedirectWhen(Func<T, bool> criteria, Func<T, string> urlSelector, QueryString queryString)
        {
            _redirectCriteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
            _urlSelector = urlSelector ?? throw new ArgumentNullException(nameof(urlSelector));
            _queryString = queryString;

            return this;
        }


        public IActionResult Build(T value)
        {
            if (_notFoundWhenNull && value == null)
            {
                return new NotFoundResult();
            }
            if (_redirectCriteria?.Invoke(value) == true)
            {
                var url = _urlSelector(value);
                if (_queryString.HasValue)
                {
                    url = url.Contains("?")
                        ? $"{url}&{_queryString.Value}"
                        : $"{url}?{_queryString.Value}";
                }

                var uri = UriHelper.Encode(new Uri(url, UriKind.Relative));
                return new RedirectResult(uri, true);
            }
            return null;
        }
    }
}