using PsychedelicExperience.Common.StructureMap;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Web.Infrastructure.Configuration;
using StructureMap;

namespace PsychedelicExperience.Web.Infrastructure
{
    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            Scan(options =>
            {
                options.TheCallingAssembly();
                options.IncludeNamespaceContainingType<GetConfigValuesHandler>();
                options.IncludeNamespaceContainingType<UserInfoResolver>();
                //options.IncludeNamespaceContainingType<TopicInteractionUpdatedHandler>();

                options.Convention<AllInterfacesConvention>();
            });
        }
    }
}