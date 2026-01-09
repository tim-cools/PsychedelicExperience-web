using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    public static class TestData
    {
        public static Center Center()
        {
            return new Center
            {
                GroupSize = new GroupSize
                {
                    Maximum = 12,
                    SingleAvailable = true
                },
                Accommodation = new Accommodation
                {
                    AirportTransfer = true,
                    DoubleRooms = 1,
                    GroupRooms = 3,
                    SingleRooms = 5
                },
                Engagement = new Engagement
                {
                    ResearchProjects = new OptionalDescription("yes, we have some")
                },
                Environment = new EnvironmentDetails
                {
                    MusicPlayed = "yes, 60's, 70's & 80's"
                },
                Facilitators = new Facilitators
                {
                    FacilitatorStay = true,
                    MaximumPerFacilitator = 8,
                    SomeoneSober = false
                },
                Location = new LocationDetails
                {
                    Description = "rabbit hole"
                },
                Medicines = new Medicines
                {
                    Ingredients = "some strange mix"
                },
                OpenSince = DateTime.Now,
                Purpose = new Purpose
                {
                    PersonalDevelopment = new OptionalDescription("Group session"),
                    ReligiousCeremonies = new OptionalDescription("Ceremonies"),

                    TreatmentOfAddictions = new OptionalDescription("Treatment OfAddictions"),
                    TreatmentOfPhysicalIllnesses = new OptionalDescription("Treatment OfPhysicalIllnesses"),
                    TreatmentOfPsychologicalDisorders = new OptionalDescription("Treatment OfPsychologicalDisorders")
                },
                Safety = new Safety
                {
                    IntakeProcess = new OptionalDescription("Intake Process"),
                    IntegrationProcess = new OptionalDescription("Integration Process"),
                    MedicalFacilitiesNearby = new OptionalDescription("MedicalFacilities Nearby"),
                    MedicalFacilitiesOnsite = new OptionalDescription("MedicalFacilities Onsite"),
                    PsychologicalTherapisOnsite = new OptionalDescription("Psychological TherapisOnsite")
                },
                Status = CenterStatus.Open,
                Team = new Team
                {
                    Description = "some shamans"
                }
            };
        }

        public static Community Community()
        {
            return new Community
            {

            };
        }

        public static HealthcareProvider HealthcareProvider()
        {
            return new HealthcareProvider
            {

            };
        }

        public static Practitioner Practitioner()
        {
            return new Practitioner
            {
                General = new General
                {
                    Background = "background",
                    Ethnicity = "Ethnicity",
                    Gender = "Gender",
                    Pronouns = "Pronouns",
                    Nationality = "Nationality",
                    Story = "Story",
                    BirthDate = DateTime.Now.Date,
                    PsychedelicExperience = "PsychedelicExperience",
                    SexualOrientation = "SexualOrientation"
                },
                Work = new Work
                {
                    Approach = "Approach",
                    Clients = "Clients",
                    Deals = "Deals",
                    Medicines = "Medicines",
                    Practices = new []{ "p1"},
                    Training = "Training",
                    Affiliations = new []{ "a1", "a2"},
                    SpiritualBackground = "SpiritualBackground",
                    StartedSince = DateTime.Now.Subtract(TimeSpan.FromDays(356 * 5)),
                    WorkPrice = WorkPrice.Expensive
                },
                Coach = new Coach
                {
                    Certifications = new []{ "c1" },
                    Experience = new CoachExperience
                    {
                        Clients = 156,
                        Hours = 4656
                    },
                    Specialities = new []{ "s1" },
                    ProfessionalBody = new []{ "b1"}
                },
                Facilitator = new Facilitator
                {
                    Sessions = 123,
                    GroupSizeMaximum = 12,
                    GroupSizeMinimum = 5,
                    Roles = new []{ "1-1" },
                    Lineage = "shipibo",
                },
                Therapist = new Therapist
                {
                    Accreditations = new []{ "a1" },
                    Specialities = new []{ "s1" }
                }
            };
        }

        public static CenterReview CenterReview()
        {
            return new CenterReview
            {
                NumberOfPeople = 2,
                NumberOfFacilitators = 2,

                Location = new Rating(ScaleOf5.One, "Location"),
                Accommodation = new Rating(ScaleOf5.Two, "Accommodation"),
                Medicine = new Rating(ScaleOf5.Three, "Medicine"),

                ExperienceId = null,
                Facilitators = new Rating(ScaleOf5.Five, "Facilitators"),

                Honest = new Rating(ScaleOf5.Four, "Honest"),
                Secure = new Rating(ScaleOf5.Two, "Secure"),
                Preparation = new Rating(ScaleOf5.Three, "Preparation"),
                BookingProcess = new Rating(ScaleOf5.Four, "BookingProcess"),
                FollowUp = new Rating(ScaleOf5.Five, "FollowUp"),
            };
        }

        public static CommunityReview CommunityReview()
        {
            return new CommunityReview
            {
                
            };
        }

        public static HealthcareProviderReview HealthcareProviderReview()
        {
            return new HealthcareProviderReview
            {
            };
        }
    }
}