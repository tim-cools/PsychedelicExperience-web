using PsychedelicExperience.Membership.Users.Domain;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Unit
{
    public class UserRoleTests
    {
        [Fact]
        public void WhenUserIsAdministrator()
        {
            var user = new User();
            user.Roles.Add(Roles.Administrator);

            user.IsAtLeast(Roles.ContentManager).ShouldBeTrue();
            user.IsAtLeast(Roles.Administrator).ShouldBeTrue();
        }

        [Fact]
        public void WhenUserIsContentManager()
        {
            var user = new User();
            user.Roles.Add(Roles.ContentManager);

            user.IsAtLeast(Roles.ContentManager).ShouldBeTrue();
            user.IsAtLeast(Roles.Administrator).ShouldBeFalse();
        }

        [Fact]
        public void WhenUserHasNoRoles()
        {
            var user = new User();

            user.IsAtLeast(Roles.ContentManager).ShouldBeFalse();
            user.IsAtLeast(Roles.Administrator).ShouldBeFalse();
        }
    }
}
