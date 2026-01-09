using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.ExperienceView;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Psychedelics.Tags.Handlers;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    internal static class Mapping
    {
        internal static OrganisationSummary MapSummary(this Organisation organisation, User user, IUserInfoResolver userInfoResolver)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            return new OrganisationSummary
            {
                OrganisationId = organisation.Id,
                Name = organisation.Name + (organisation.Removed ? " (removed)" : null),
                Description = organisation.Description.SmartTruncate(200),

                Community = Map(organisation.Community),
                Practitioner = Map(organisation.Practitioner),
                Center = Map(organisation.Center),
                HealthcareProvider = Map(organisation.HealthcareProvider),

                Url = organisation.GetUrl(),

                Person = organisation.Person,
                Types = organisation.Types.ToArray(),
                Position = organisation.Address?.Position,
                Photos = Map(organisation.Photos),
                ReviewsCount = organisation.Reviews.Count,
                ReviewsRating = organisation.Reviews.Count > 0
                    ? decimal.Round(organisation.Reviews.Rating / (decimal) organisation.Reviews.Count, 1)
                    : (decimal?)null,
                ReportsCount = user.SeesOrganisationsReports()
                    ? organisation.Reports.Count
                    : (int?)null
            };
        }

        private static CommunitySummary Map(Community community)
        {
            return community != null ? new CommunitySummary() : null;
        }

        private static PractitionerSummary Map(Practitioner practitioner)
        {
            return practitioner != null ? new PractitionerSummary() : null;
        }

        private static CenterSummary Map(Center center)
        {
            return center != null ? new CenterSummary
            {
                GroupSize = center.GroupSize?.Maximum,
                Status = center.Status.ToString(),
                OpenSince = center.OpenSince
            } : null;
        }

        private static HealthcareProviderSummary Map(HealthcareProvider healthcareProvider)
        {
            return healthcareProvider != null ? new HealthcareProviderSummary() : null;
        }

        internal static OrganisationSummary MapSummary(this Organisation organisation, User user, TopicInteraction interaction,
            IUserInfoResolver userInfoResolver)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            return new OrganisationSummary
            {
                OrganisationId = organisation.Id,
                Name = $"{organisation.Name}{(organisation.Removed ? " (removed)" : null)}",
                Description = organisation.Description.SmartTruncate(200),
                Person = organisation.Person,

                Community = Map(organisation.Community),
                Practitioner = Map(organisation.Practitioner),
                Center = Map(organisation.Center),
                HealthcareProvider = Map(organisation.HealthcareProvider),

                Url = organisation.GetUrl(),

                Types = organisation.Types.ToArray(),
                Position = organisation.Address?.Position,
                Photos = Map(organisation.Photos),
                Followers = interaction?.Followers ?? 0,
                Likes = interaction?.Likes ?? 0,
                Dislikes = interaction?.Dislikes ?? 0,
                Views = interaction?.Views ?? 0,
                ReviewsCount = organisation.Reviews.Count,
                ReviewsRating = organisation.Reviews.Count > 0
                    ? decimal.Round(organisation.Reviews.Rating / (decimal) organisation.Reviews.Count, 1)
                    : (decimal?)null,
                ReportsCount = user.SeesOrganisationsReports()
                    ? organisation.Reports.Count
                    : (int?)null
            };
        }

        internal static OrganisationDetails MapDetails(this Organisation organisation, User user,
            IUserInfoResolver userInfoResolver, IDictionary<Guid, Organisation> relatedOrganisations)
        {
            return organisation.MapDetails(user, new List<Review>(), relatedOrganisations,  userInfoResolver);
        }

        internal static OrganisationDetails MapDetails(this Organisation organisation, User user, IReadOnlyList<Review> reviews, IDictionary<Guid, Organisation> relatedOrganisations, IUserInfoResolver userInfoResolver)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            if (!organisation.CanView(user))
            {
                return null;
            }

            var canEditContent = user?.IsAtLeast(Roles.ContentManager) == true;
            var isOwner = organisation.IsOwner(user);

            var privileges = new OrganisationDetailsPrivileges
            {
                Manageable = canEditContent,
                Editable = isOwner || canEditContent,
                IsOwner = isOwner
            };

            return new OrganisationDetails
            {
                OrganisationId = organisation.Id,
                Person = organisation.Person,
                Types = organisation.Types.ToArray(),
                Name = organisation.Name,
                Description = organisation.Description,
                ExternalDescription = ExternalDescription(organisation),
                Slug = organisation.Slug(),
                Url = organisation.GetUrl(),

                Community = MapCommunity(organisation.Community),
                Practitioner = MapPractitioner(organisation.Practitioner),
                Center = MapCenter(organisation.Center),
                HealthcareProvider = MapHealthcareProvider(organisation.HealthcareProvider),

                Owners = Map(privileges, organisation.Owners, userInfoResolver),

                Contacts = Map(organisation.Contacts),
                Address = MapAddress(organisation),
                Tags = organisation.Tags.Select(tag => tag).ToArray(),
                Reports = MapReports(organisation, user, userInfoResolver),
                Privileges = privileges,
                Info = Map(organisation.Info),
                Warning = Map(organisation.Warning),
                Photos = Map(organisation.Photos),

                ReviewsCount = organisation.Reviews.Count,
                ReviewsRating = organisation.Reviews.Count > 0
                    ? decimal.Round(organisation.Reviews.Rating / (decimal) organisation.Reviews.Count, 1)
                    : (decimal?)null,
                Reviews = MapReviews(reviews, userInfoResolver),
                Relations = MapRelations(organisation.Relations, relatedOrganisations),
                RelationsFrom = MapRelations(organisation.RelationsFrom, relatedOrganisations),
            };
        }

        private static OrganisationRelation[] MapRelations(IList<Relation> relations,
            IDictionary<Guid, Organisation> relatedOrganisations)
        {
            if (relatedOrganisations == null) return new OrganisationRelation[0];

            return relations.Select(relation => MapRelation(relation, relatedOrganisations)).ToArray();
        }

        private static OrganisationRelation MapRelation(Relation relation, IDictionary<Guid, Organisation> relatedOrganisations)
        {
            if (!relatedOrganisations.TryGetValue(relation.TargetId, out var relatedOrganisation))
            {
                throw new InvalidOperationException($"Organisation not found: {relation.TargetId}");
            }

            return new OrganisationRelation
            {
                OrganisationId = relation.TargetId,
                OrganisationName = relatedOrganisation.Name,
                OrganisationUrl = relatedOrganisation.GetUrl(),
                Relation = relation.Name,
                Photo = relatedOrganisation.Photos.Count > 0 ? new ShortGuid(relatedOrganisation.Photos[0].PhotoId) : null
            };
        }

        private static CommunityDetails MapCommunity(Community community)
        {
            if (community == null) return null;

            return new CommunityDetails
            {

            };
        }

        private static PractitionerDetails MapPractitioner(Practitioner practitioner)
        {
            if (practitioner == null) return null;

            return new PractitionerDetails
            {
                General = practitioner.General,
                Work = practitioner.Work,
                Therapist = practitioner.Therapist,
                Coach = practitioner.Coach,
                Facilitator = practitioner.Facilitator,
            };
        }

        private static CenterDetails MapCenter(Center center)
        {
            if (center == null) return null;

            return new CenterDetails
            {
                Status = center.Status.ToString(),
                OpenSince = center.OpenSince,
                Accommodation = center.Accommodation,
                Engagement = center.Engagement,
                Environment = center.Environment,
                Facilitators = center.Facilitators,
                GroupSize = center.GroupSize,
                Location = center.Location,
                Medicines = center.Medicines,
                Purpose = center.Purpose,
                Safety = center.Safety,
                Team = center.Team
            };
        }

        private static HealthcareProviderDetails MapHealthcareProvider(HealthcareProvider healthcare)
        {
            if (healthcare == null) return null;

            return new HealthcareProviderDetails
            {

            };
        }

        private static Owner[] Map(OrganisationDetailsPrivileges privileges, IList<Guid> owners, IUserInfoResolver userInfoResolver)
        {
            if (!privileges.Editable) return null;

            return owners.Select(value => new Owner
                {
                    UserId = value,
                    UserName = userInfoResolver.GetInfo((UserId) value).DisplayName
                }).ToArray();
        }

        internal static OrganisationReviewResult MapDetails(this Review review, User user, Organisation organisation, Experience experience, IUserInfoResolver userInfoResolver)
        {
            if (review == null) throw new ArgumentNullException(nameof(review));
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            var owner = userInfoResolver.GetInfo((UserId) review.UserId);

            var canEditContent = user?.IsAtLeast(Roles.ContentManager) == true;
            if (!canEditContent && review.Removed)
            {
                return null;
            }
            
            var isOwner = organisation.IsOwner(user);

            var privileges = new OrganisationDetailsPrivileges
            {
                Manageable = canEditContent,
                Editable = isOwner || canEditContent,
                IsOwner = isOwner
            };

            var result = new OrganisationReviewDetails
            {
                OrganisationId = organisation.Id,
                ReviewId = review.Id,

                Community = MapCommunityReview(review.Community),
                Center = MapCenterReview(review.Center),
                HealthcareProvider = MapHealthcareProviderReview(review.HealthcareProvider),
                Experience = MapExperience(experience),

                Rating = review.Rating,
                Visited = review.Visited,
                Created = review.Created,
                Privileges = privileges,
                Name = review.Name,
                UserName = owner.DisplayName,
                UserId = review.UserId,
                Description = review.Description,
                ExternalDescription = ExternalDescription(review),
                Slug = review.Slug(),
                Url = review.GetUrl(),
            };

            return new OrganisationReviewResult
            {
                Organisation = organisation.MapDetails(user, userInfoResolver, null),
                Review = result
            };
        }

        private static ReviewExperienceSummary MapExperience(Experience experience)
        {
            return experience != null
                ? new ReviewExperienceSummary
                {
                    ExperienceId = experience.Id,
                    Description = experience.PublicDescription,
                    Title = experience.Title,
                    Url = GetUrl(experience)
                }
                : null;
        }

        private static string GetUrl(Experience experience)
        {
            return $"/experience/{new ShortGuid(experience.Id)}/{experience.Slug()}";
        }

        private static CommunityReviewDetails MapCommunityReview(CommunityReview review)
        {
            if (review == null) return null;

            return new CommunityReviewDetails
            {
            };
        }

        private static CenterReviewDetails MapCenterReview(CenterReview review)
        {
            if (review == null) return null;

            return new CenterReviewDetails
            {
                Facilitators = MapRating(review.Facilitators),
                Accommodation = MapRating(review.Accommodation),
                BookingProcess = MapRating(review.BookingProcess),
                FollowUp = MapRating(review.FollowUp),
                Honest = MapRating(review.Honest),
                Location = MapRating(review.Location),
                Medicine = MapRating(review.Medicine),
                Preparation = MapRating(review.Preparation),
                Secure = MapRating(review.Secure),

                ExperienceId = review.ExperienceId?.Value,
                
                NumberOfPeople = review.NumberOfPeople,
                NumberOfFacilitators = review.NumberOfFacilitators
            };
        }

        private static RatingDetails MapRating(Rating rating)
        {
            return rating != null ? new RatingDetails
            {
                Description = rating.Description,
                Value = (int) rating.Value
            } : null;
        }

        private static HealthcareProviderReviewDetails MapHealthcareProviderReview(HealthcareProviderReview review)
        {
            if (review == null) return null;

            return new HealthcareProviderReviewDetails
            {

            };
        }

        private static string ExternalDescription(Organisation organisation)
        {
            var substances = organisation.Tags.Where(tag => TagRepository.IsInCategory(TagsDomain.Organisations, "Medicine", tag, false))
                .Distinct()
                .Aggregate((string)null, (result, item) => result == null ? item : $"{result}, {item}");

            if (substances != null)
            {
                substances = substances + " - ";
            }

            return organisation.Warning != null
                ? $"{substances}Warning: {organisation.Warning.Title}".SmartTruncate(Seo.ExternalDescriptionLength)
                : $"{substances}{organisation.Description}".SmartTruncate(Seo.ExternalDescriptionLength);
        }

        private static string ExternalDescription(Review review)
        {
            return review.Description.SmartTruncate(Seo.ExternalDescriptionLength);
        }

        internal static OrganisationMapPoint MapMapPoint(this Organisation organisation)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            return new OrganisationMapPoint
            {
                Id = organisation.Id,
                Position = organisation.Address.Position
            };
        }

        private static ContactDetail[] Map(IList<Contact> organisationContacts)
        {
            var emailSet = false;

            var result = new List<ContactDetail>();
            foreach (var detail in organisationContacts.Select(Map))
            {
                /* if (detail.Type == ContactTypes.EMail)
                {
                    if (emailSet)
                    {
                        continue;
                    }
                    emailSet = true;
                    detail.Value = "****";
                }*/

                result.Add(detail);
            }
            return result.ToArray();
        }

        private static ContactDetail Map(Contact organisationContact)
        {
            return new ContactDetail
            {
                Type = organisationContact.Type,
                Value = organisationContact.Value
            };
        }

        private static PhotoSummary[] Map(IList<Photo> organisationPhotos)
        {
            return organisationPhotos
                .Select(Map)
                .ToArray();
        }

        private static OrganisationDetailsInfo Map(OrganisationInfo organisationInfo)
        {
            return organisationInfo != null
                ? new OrganisationDetailsInfo
                {
                    Title = organisationInfo.Title,
                    Content = organisationInfo.Content
                }
                : null;
        }

        private static Messages.Organisations.Queries.OrganisationAddress MapAddress(Organisation organisation)
        {
            return organisation.Address != null
                ? new Messages.Organisations.Queries.OrganisationAddress
                {
                    Name = organisation.Address.Name,
                    Position = organisation.Address.Position,
                    PlaceId = organisation.Address.PlaceId
                }
                : null;
        }

        private static ReportDetails[] MapReports(Organisation organisation, User user, IUserInfoResolver userInfoResolver)
        {
            return user.SeesOrganisationsReports()
                ? organisation.Reports.Select(report => CreateReportDetails(userInfoResolver, report)).ToArray()
                : null;
        }

        private static OrganisationsReviewSummary[] MapReviews(IReadOnlyList<Review> reviews, IUserInfoResolver userInfoResolver)
        {
            return reviews != null
                ? reviews.Select(review => CreateReview(userInfoResolver, review)).ToArray()
                : null;
        }

        private static OrganisationsReviewSummary CreateReview(IUserInfoResolver userInfoResolver, Review review)
        {
            return new OrganisationsReviewSummary
            {
                ReviewId = review.Id,
                Visited = review.Visited,
                Name = review.Name,
                Description = review.Description,
                Slug = review.Slug(),
                Rating = review.Rating
            };
        }

        private static ReportDetails CreateReportDetails(IUserInfoResolver userInfoResolver, Report report)
        {
            if (report == null)
            {
                return null;
            }
            
            var userInfo = userInfoResolver.GetInfo((UserId)report.UserId);
            return new ReportDetails
                {
                    DateTime = report.DateTime,
                    Reason = report.Reason,
                    UserId = report.UserId,
                    UserName = userInfo?.DisplayName
                };
        }

        internal static PhotoSummary Map(this Photo photo)
        {
            return photo != null 
                ? new PhotoSummary
                {
                    Id = photo.PhotoId
                }
                : null;
        }

        internal static PhotoDetails MapDetails(this Photo photo)
        {
            return photo != null 
                ? new PhotoDetails
                {
                    Id = photo.PhotoId,
                    FileName = photo.FileName
                } 
                : null;
        }
    }

    public static class UserExtensions
    {
        public static bool SeesOrganisationsReports(this User user)
        {
            return user != null && user.IsAtLeast(Roles.ContentManager);
        }
    }
}