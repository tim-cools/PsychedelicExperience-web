using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Research
{
    public class GenerateCommandHandlerTemplate
    {
        [Fact]
        public void Generate()
        {
            var commandTypes = GetCommandTypes();

            GenerateTests(commandTypes);
        }

        private static Type[] GetCommandTypes()
        {
            var commandType = typeof(AddEvent);
            var commandTypes = commandType.GetTypeInfo().Assembly.GetTypes()
                .Where(type => type.Namespace == commandType.Namespace)
                .OrderBy(type => type.Name)
                .ToArray();
            return commandTypes;
        }

        private void GenerateTests(Type[] commandTypes)
        {
            var builder = new StringBuilder();
            foreach (var commandType in commandTypes)
            {
                GenerateTest(builder, commandType);
            }

            File.WriteAllText(@"c:\temp\tetst.cs", builder.ToString());
        }

        private void GenerateTest(StringBuilder builder, Type commandType)
        {
            var aggregateType = GetAggregateType(commandType);
            var idProperty = aggregateType + "Id";
            var idField = "_" + idProperty.Substring(0, 1).ToLowerInvariant() + idProperty.Substring(1);
            var eventName = EventName(commandType.Name);

            builder.AppendLine($@"      public class When{commandType.Name} : ServiceTestBase<IMediator>, IClassFixture <PsychedelicsIntegrationTestFixture>");
            builder.AppendLine($@"    {{
        private EventId _eventId;");
            if (idField != "_eventId")
            {
                builder.AppendLine($@"private readonly Guid {idField} = Guid.NewGuid();");
            }
            builder.AppendLine($@"        private {commandType.Name} _command;
        private UserId _userId;
        private Result _result;
");
            builder.AppendLine($@"      public When{commandType.Name}(PsychedelicsIntegrationTestFixture fixture)");
            builder.AppendLine(@"            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

");
            builder.AppendLine($@"      _command = new {commandType.Name}(_userId, {idField});");
            builder.AppendLine(@"_result = context.Service.ExecuteNowWithTimeout(_command);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {");
            builder.AppendLine($@"
                var aggregate = context.Session.Load<{aggregateType}>({idField});");
            builder.AppendLine(@"
                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                ");
            builder.AppendLine($@"var events = context.Session.LoadEvents({idField}).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<{eventName}>();

                @event.UserId.ShouldBe(_userId);
                ");
            builder.AppendLine($@"@event.{idProperty}.ShouldBe({idField});");

            foreach (var propertyInfo in commandType.GetProperties())
            {
                if (propertyInfo.Name == "EventId" || propertyInfo.Name == "UserId") continue;

                builder.AppendLine($@"@event.{propertyInfo.Name}.ShouldBe(_command.{propertyInfo.Name});");
            }

            builder.AppendLine(@"
            });
        }
    }");
            builder.AppendLine();
        }

        private string EventName(string commandTypeName)
        {
            var words = Regex.Replace(
                Regex.Replace(
                    commandTypeName,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            ).Split(' ');

            var verb = words.First();
            return String.Join("", words.Skip(1).Union(new[] { verb, verb.EndsWith("e") ? "d" : "ed" }).ToArray());
        }

        private string GetAggregateType(Type commandType)
        {
            return "Event";
        }
    }
}
