using System.Collections.Generic;
using System.Xml.Linq;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Web.Infrastructure.Security;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Infrastructure.Security
{
    public class WhenStoringXmlDocuments : ServiceTestBase<DatabaseXmlRepository>,
        IClassFixture<WebIntegrationTestFixture>
    {
        private IReadOnlyCollection<XElement> _elements;

        public WhenStoringXmlDocuments(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<DatabaseXmlRepository> context)
        {
            StoreElement(context, 1);
            StoreElement(context, 2);
            StoreElement(context, 3);
        }

        private void StoreElement(TestContext<DatabaseXmlRepository> context, int index)
        {
            context.Service.StoreElement(Element(index), "element" + index);
        }

        private XElement Element(int index)
        {
            return new XElement("Element" + index);
        }

        protected override void When(TestContext<DatabaseXmlRepository> context)
        {
            _elements = context.Service.GetAllElements();
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenAllElementsShouldBeReturned()
        {
            _elements.Count.ShouldBe(3);

            _elements.ShouldContain(element =>  element.Name == "Element1");
            _elements.ShouldContain(element =>  element.Name == "Element2");
            _elements.ShouldContain(element =>  element.Name == "Element3");
        }
    }
}