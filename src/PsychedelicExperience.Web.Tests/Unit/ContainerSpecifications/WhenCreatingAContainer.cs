using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests.Unit;
using PsychedelicExperience.Common.Tests.Unit.ContainerSpecifications;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Configuration;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Unit.ContainerSpecifications
{
    public class WhenCreatingAContainer
    {
        [Fact]
        public void ThenTheDocumentMessageHandlerShouldBeGettable()
        {
            var container = TestContainerFactory.CreateContainer(config =>
            {
                config.AddRegistry<WebRegistry>();
                config.For<IConfiguration>().Use<DummyConfiguration>();
                config.For<IHostingEnvironment>().Use<DummyHostingEnvironment>();
            });

            Debug.WriteLine(container.WhatDidIScan());

            container.GetInstance<IAsyncRequestHandler<GetConfigValuesQuery, IDictionary<string, object>>>()
                .ShouldNotBeNull();
        }
    }

    public class DummyHostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
