using System;
using System.Text;
using MemBus.Support;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisations : IRequest<OrganisationsResult>
    {
        public UserId UserId { get; }
        public string[] Types { get; }
        public string Query { get; }
        public string[] Tags { get; }
        public string Country { get; }
        public int Page { get; }
        public bool OnlyWithoutTags { get; }
        public bool FilterByUser { get; }
        public bool? HasOwner { get; }
        public Format Format { get; }

        public GetOrganisations(UserId userId, string country = null, string query = null, string[] tags = null,
            int page = 0, bool onlyWithoutTags = false, bool filterByUser = false, string[] types = null,
            bool? hasOwner = null, Format format = Format.Json)
        {
            UserId = userId;
            Types = types;
            Country = country;
            Query = query;
            Tags = tags;
            Page = page;

            OnlyWithoutTags = onlyWithoutTags;
            FilterByUser = filterByUser;
            HasOwner = hasOwner;
            Format = format;
        }
    }

    public class OrganisationCsv
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
    }
}