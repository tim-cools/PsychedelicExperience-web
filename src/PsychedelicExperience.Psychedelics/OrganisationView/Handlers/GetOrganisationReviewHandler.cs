using System;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationReviewHandler : QueryHandler<GetOrganisationReview, OrganisationReviewResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetOrganisationReviewHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<OrganisationReviewResult> Execute(GetOrganisationReview query)
        {
            if (query.ReviewId == null) throw new InvalidOperationException("ReviewId is null");
            if (query.OrganisationId == null) throw new InvalidOperationException("OrganisationId is null");

            var id = (Guid) query.OrganisationId;

            var organisation = await Session.LoadAsync<Organisation>(id);
            var review = await Session.LoadAsync<Review>(query.ReviewId.Value);
            var experience = review?.ExperienceId != null
                ? await Session.LoadAsync<ExperienceView.Experience>(review.ExperienceId.Value)
                : null;

            var user = await Session.LoadUserAsync(query.UserId);

            return organisation != null 
                ? review?.MapDetails(user, organisation, experience, _userInfoResolver) 
                : null;
        }
    }
}
