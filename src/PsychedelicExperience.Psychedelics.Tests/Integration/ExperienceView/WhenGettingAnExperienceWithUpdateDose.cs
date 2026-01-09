using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.ExperienceView
{
    public class WhenGettingAnExperienceWithUpdateDose : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private ExperienceDetails _result;
        private ExperienceId _experienceId;
        private DoseId _doseId;

        public WhenGettingAnExperienceWithUpdateDose(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _experienceId = context.AddExperience(_userId);
            _doseId = context.AddDose(_userId, _experienceId);

            context.UpdateDoseSubstance(_userId, _doseId, "5-meo-dmt");
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetExperience(_userId, _experienceId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenExperiencesShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.ExperienceId.ShouldBe((ShortGuid) _experienceId);
            _result.Doses.Length.ShouldBe(1);
            _result.Doses[0].Substance.ShouldBe("5-meo-dmt");
        }
    }
}