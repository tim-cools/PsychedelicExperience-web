using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using SimpleMvcSitemap;

namespace PsychedelicExperience.Web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ISitemapProvider _sitemapProvider;

        public SitemapController(IMediator mediator, ISitemapProvider sitemapProvider)
        {
            _mediator = mediator;
            _sitemapProvider = sitemapProvider;
        }

        public ActionResult Index()
        {
            var sitemapIndexNodes = new List<SitemapIndexNode>
            {
                //new SitemapIndexNode(Url.Action("Home", "Sitemap")),
                //new SitemapIndexNode(Url.Action("Experiences", "Sitemap")),
                new SitemapIndexNode(Url.Action("Organisations", "Sitemap")),
                //new SitemapIndexNode(Url.Action("Documents", "Sitemap")),
                new SitemapIndexNode(Url.Action("OrganisationReviews", "Sitemap")),
                new SitemapIndexNode(Url.Action("Events", "Sitemap")),
                new SitemapIndexNode("/c/sitemap_index.xml")
            };

            return _sitemapProvider.CreateSitemapIndex(new SitemapIndexModel(sitemapIndexNodes));
        }

        public ActionResult Home()
        {
            var nodes = new List<SitemapNode>
            {
                new SitemapNode("/"),
                new SitemapNode("/about"),
                new SitemapNode("/privacy"),
                new SitemapNode("/get-involved"),
                new SitemapNode("/legal")
            };

            return _sitemapProvider.CreateSitemap(new SitemapModel(nodes));
        }

        public async Task<ActionResult> Experiences()
        {
            var result = await _mediator.Send(new GetExperiencesSitemap());

            var nodes = new List<SitemapNode>
            {
                new SitemapNode(Url.Action("Get", "ExperienceViews")),
            };

            AddNodes(nodes, result.Substances, "GetBySubstance", "ExperienceViews", value => new RouteValueDictionary { { "substance", value}});
            AddNodes(nodes, result.Tags, "GetByTag", "ExperienceViews", value => new RouteValueDictionary { { "tag", value}});
            AddNodes(nodes, result.Experiences, "GetById", "ExperienceViews", value => new RouteValueDictionary { { "id", new ShortGuid(value.Id)}, {"slug", value.Slug() } });

            return _sitemapProvider.CreateSitemap(new SitemapModel(nodes));
        }

        public async Task<ActionResult> Organisations()
        {
            const string controllerName = "OrganisationViews";

            var result = await _mediator.Send(new GetOrganisationsSitemap());

            var nodes = new List<SitemapNode>();
            nodes.AddRange(result.Types.Select(type => new SitemapNode(Url.Content($"/{type}"))));
            nodes.AddRange(result.TypesCountries.Select(type => new SitemapNode(Url.Content($"/{type.Type}/country/{type.Country}"))));
            nodes.AddRange(result.TypesTags.Select(type => new SitemapNode(Url.Content($"/{type.Type}/tag/{WebUtility.UrlEncode(type.Tag)}"))));
            nodes.AddRange(result.TypesTagsCountries.Select(type => new SitemapNode(Url.Content($"/{type.Type}/country/{type.Country}/tag/{WebUtility.UrlEncode(type.Tag)}"))));

            AddNodes(nodes, result.Organisations, nameof(OrganisationViewsController.GetById), controllerName, value => new RouteValueDictionary { { "id", new ShortGuid(value.Id) }, { "slug", value.Slug() } });

            return _sitemapProvider.CreateSitemap(new SitemapModel(nodes));
        }
        
        
        public async Task<ActionResult> OrganisationReviews()
        {
            const string controllerName = "OrganisationViews";

            var result = await _mediator.Send(new GetOrganisationReviewsSitemap());

            var nodes = new List<SitemapNode>();

            AddNodes(nodes, result.Reviews, "GetReviewByIdAndSlug", controllerName, value => new RouteValueDictionary
            {
                { "reviewId", new ShortGuid(value.Id) },
                { "id", new ShortGuid(value.OrganisationId) },
                { "slug", value.Slug() }
            });

            return _sitemapProvider.CreateSitemap(new SitemapModel(nodes));
        }

        public async Task<ActionResult> Events()
        {
            const string controllerName = "EventViews";

            var result = await _mediator.Send(new GetEventsSitemap());

            var nodes = new List<SitemapNode>
            {
                new SitemapNode(Url.Action("Get", controllerName)),
            };

            AddNodes(nodes, result.Events, nameof(EventViewsController.GetByIdAndSlug), controllerName, value => new RouteValueDictionary { { "id", new ShortGuid(value.Id)}, {"slug", value.Slug() } });
            AddNodes(nodes, result.Countries, nameof(EventViewsController.GetByCountry), controllerName, value => new RouteValueDictionary { { "country", value } });
            AddNodes(nodes, result.EventTypes, nameof(EventViewsController.GetByType), controllerName, value => new RouteValueDictionary { { "eventType", value } });
            AddNodes(nodes, result.CountriesAndTypes, nameof(EventViewsController.GetByCountryAndType), controllerName, value => new RouteValueDictionary { { "country", value.County }, { "eventType", value.EventType }  });

            return _sitemapProvider.CreateSitemap(new SitemapModel(nodes));
        }

        public async Task<ActionResult> Documents()
        {
            const string controllerName = "DocumentViews";

            var result = await _mediator.Send(new GetDocumentsSitemap());

            var nodes = new List<SitemapNode>
            {
                new SitemapNode(Url.Action("Get", controllerName)),
            };

            AddNodes(nodes, result.Tags, "GetByTag", controllerName, value => new RouteValueDictionary { { "tag", value } });
            AddDocumentNodes(nodes, result.Documents, "GetBySlug", controllerName, slug => !slug.Contains("/") ? slug : null);
            AddDocumentNodes(nodes, result.Documents, "GetBlogBySlug", controllerName, slug => slug.StartsWith("blog/") ? slug.Substring("blog/".Length) : null);
            AddDocumentNodes(nodes, result.Documents, "GetSupportBySlug", controllerName, slug => slug.StartsWith("support/") ? slug.Substring("support/".Length) : null);

            return _sitemapProvider.CreateSitemap(new SitemapModel(nodes));
        }

        private void AddDocumentNodes(List<SitemapNode> nodes, IEnumerable<DocumentsSitemapEntry> items, string actionName, string controllerName, Func<string,string> predicate)
        {
            var newNodes = items
                    .Where(item => item.Slug != null)
                    .Select(item => predicate(item.Slug))
                    .Where(slug => slug != null)
                    .Select(slug =>
                    {
                        var value = new RouteValueDictionary { { "slug", slug } };
                        return new SitemapNode(Url.Action(actionName, controllerName, value));
                    });
            nodes.AddRange(newNodes);
        }

        private void AddNodes<T>(List<SitemapNode> nodes, IEnumerable<T> items, string actionName, string controllerName, Func<T, RouteValueDictionary> routeFactory)
        {
            var newNodes = items.Select(item => new SitemapNode(Url.Action(actionName, controllerName, routeFactory(item))));
            nodes.AddRange(newNodes);
        }
    }
}