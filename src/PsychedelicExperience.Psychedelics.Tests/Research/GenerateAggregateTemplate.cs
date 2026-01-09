using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Research
{
    public class GenerateAggregateTemplate
    {
        [Fact]
        public void Generate()
        {
            var commandTypes = GetCommandTypes();
            var eventTypes = GetEventTypes();

            GenerateTests(commandTypes, eventTypes);
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

        private static Type[] GetEventTypes()
        {
            var commandType = typeof(EventAdded);
            var commandTypes = commandType.GetTypeInfo().Assembly.GetTypes()
                .Where(type => type.Namespace == commandType.Namespace)
                .ToArray();
            return commandTypes;
        }

        private void GenerateTests(Type[] commandTypes, Type[] eventTypes)
        {
            var builder = new StringBuilder();
            foreach (var commandType in commandTypes)
            {
                GenerateHandler(builder, commandType);
            }
            foreach (var eventType in eventTypes)
            {
                GenerateApplier(builder, eventType);
            }

            File.WriteAllText(@"c:\temp\agg.cs", builder.ToString());
        }

        //public void Handle(User user, ReportOrganisation command)
        //{
        //    Publish(new OrganisationReported
        //    {
        //        OrganisationId = (OrganisationId)Id,
        //        UserId = new UserId(user.Id),
        //        Reason = command.Reason
        //    });
        //}

        //public void Apply(OrganisationReported @event)
        //{
        //}

        private void GenerateHandler(StringBuilder builder, Type commandType)
        {
            builder.AppendLine($@"              public void Handle(User user, {commandType.Name} command)");
            builder.AppendLine(@"              {");
            builder.AppendLine($@"              Publish(new {EventName(commandType.Name)}");
            builder.AppendLine(@"              {");
            builder.AppendLine($@"                  EventId = (EventId)Id,");
            builder.AppendLine($@"                  UserId = new UserId(user.Id),");

            foreach (var propertyInfo in commandType.GetProperties())
            {
                if (propertyInfo.Name == "EventId" || propertyInfo.Name == "UserId") continue;
                
                builder.AppendLine($@"                  {propertyInfo.Name} = command.{propertyInfo.Name},");
            }

            builder.AppendLine(@"              });");
            builder.AppendLine(@"                      }");
            builder.AppendLine(@"");
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

            return String.Join("", words.Skip(1).Union(new[] {words.First() + "ed"}).ToArray());
        }

        private void GenerateApplier(StringBuilder builder, Type commandType)
        {
            builder.AppendLine($@"              public void Apply({commandType.Name} @event)");
            builder.AppendLine(@"              {");

            foreach (var propertyInfo in commandType.GetProperties())
            {
                if (propertyInfo.Name == "EventId" || propertyInfo.Name == "UserId" || propertyInfo.Name == "EventTimestamp")
                    continue;

                builder.AppendLine($@"                  {propertyInfo.Name} = @event.{propertyInfo.Name};");
            }

            builder.AppendLine(@"               }");
            builder.AppendLine(@"");
        }
    }
}
