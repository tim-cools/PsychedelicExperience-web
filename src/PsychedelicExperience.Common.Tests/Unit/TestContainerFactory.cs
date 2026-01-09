using System;
using System.Diagnostics;
using StructureMap;

namespace PsychedelicExperience.Common.Tests.Unit
{
    public static class TestContainerFactory
    {
        public static IContainer CreateContainer(Action<ConfigurationExpression> extraConfig = null)
        {
            var container = new Container(config =>
            {
                config.AddRegistry<TestCommonRegistry>();

                extraConfig?.Invoke(config);
                config.AddRegistry<TestRegistry>();

                config.For<IContext>().Use(context => context);
            });

            Debug.WriteLine(container.WhatDidIScan());

            return container;
        }
    }
}