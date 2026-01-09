using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Unit
{
    public class DocumentTags : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private Tag[] _result;

        public DocumentTags(PsychedelicsIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new TagsQuery(null, null, TagsDomain.Experiences);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenMessageHandlerShouldBeGettable()
        {
            var builder = new StringBuilder();
            foreach (var category in _result.GroupBy(cat => cat.Category))
            {
                builder.AppendLine("Category: " + category.Key);

                foreach (var subCategory in category.GroupBy(cat => cat.SubCategory))
                {
                    if (!string.IsNullOrWhiteSpace(subCategory.Key))
                    {
                        builder.AppendLine("Sub Category: " + subCategory.Key);
                    }

                    foreach (var tag in subCategory)
                    {
                        builder.AppendLine("- " + tag.Name);
                    }
                }
                builder.AppendLine("");
            }
            File.WriteAllText("c:\\temp\\tags.log", builder.ToString());
        }
    }
}
