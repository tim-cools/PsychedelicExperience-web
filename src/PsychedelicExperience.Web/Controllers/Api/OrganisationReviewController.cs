using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class OrganisationReviewController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public OrganisationReviewController(IMediator messageDispatcher)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
        }

        [HttpGet("~/api/organisation/{organisationId}/review/")]
        [ProducesResponseType(typeof(OrganisationReviewsResult), 200)]
        public async Task<IActionResult> GetList(string query, string country, string[] tags, int page = 0)
        {
            var userId = User.GetUserId();

            var organisationsQuery = new GetOrganisationReviews(userId, query, tags, page);
            var organisations = await _messageDispatcher.Send(organisationsQuery);

            return Json(organisations);
        }

        [HttpGet("~/api/organisation/{organisationId}/review/{reviewId}/")]
        [ProducesResponseType(typeof(OrganisationReviewDetails), 200)]
        public async Task<IActionResult> Get(OrganisationId organisationId, OrganisationReviewId reviewId)
        {
            var userId = User.GetUserId();

            var query = new GetOrganisationReview(userId, organisationId, reviewId);
            var result = await _messageDispatcher.Send(query);

            return result != null ? Json(result) : reviewId.AcceptedWhenRecent();
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{organisationId}/review/{reviewId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Remove(OrganisationId organisationId, OrganisationReviewId reviewId)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationReview(reviewId, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/report")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Report(OrganisationId organisationId, OrganisationReviewId reviewId, string reason)
        {
            var userId = User.GetUserId();

            var query = new ReportOrganisationReview(organisationId, reviewId, userId, reason);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/update-name")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateName(OrganisationId organisationId, OrganisationReviewId reviewId, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationReviewName(reviewId, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/update-description")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateDescription(OrganisationId organisationId, OrganisationReviewId reviewId, string description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationReviewDescription(reviewId, userId, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/rate")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Rate(OrganisationId organisationId, OrganisationReviewId reviewId, ScaleOf5 rating)
        {
            var userId = User.GetAuthorizedUserId();

            var command = new RateOrganisationReview(reviewId, userId, rating);

            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/center")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Rate(OrganisationId organisationId, OrganisationReviewId reviewId, CenterReview center)
        {
            var userId = User.GetAuthorizedUserId();

            var command = new ChangeOrganisationReviewCenter(reviewId, userId, center);

            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/community")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Rate(OrganisationId organisationId, OrganisationReviewId reviewId, CommunityReview community)
        {
            var userId = User.GetAuthorizedUserId();

            var command = new ChangeOrganisationReviewCommunity(reviewId, userId, community);

            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{organisationId}/review/{reviewId}/healthcare-provider")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Rate(OrganisationId organisationId, OrganisationReviewId reviewId, HealthcareProviderReview healthcareProvider)
        {
            var userId = User.GetAuthorizedUserId();

            var command = new ChangeOrganisationReviewHealthcareProvider(reviewId, userId, healthcareProvider);

            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }
    }
}
