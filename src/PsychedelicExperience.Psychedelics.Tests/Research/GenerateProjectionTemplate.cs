using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Research
{
    public class GenerateProjectionTemplate
    {
        const string ProjectionName = "EventProjection";
        const string ViewType = "Event";

        [Fact]
        public void Generate()
        {
            var commandTypes = GetCommandTypes();

            GenerateTests(commandTypes);
        }

        private static Type[] GetCommandTypes()
        {
            var eventType = typeof(OrganisationAdded);
            var eventTypes = eventType.GetTypeInfo().Assembly.GetTypes()
                .Where(type => type.Namespace == eventType.Namespace)
                .ToArray();
            return eventTypes;
        }

        private void GenerateTests(Type[] eventTypes)
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"      public class {ProjectionName} : ViewProjection<{ViewType}>");
            builder.AppendLine(@"    {");

            GenerateConstuctor(builder, eventTypes);

            foreach (var eventType in eventTypes)
            {
                GenerateProjector(builder, eventType);
            }

            builder.AppendLine(@"    }");
            builder.AppendLine();

            File.WriteAllText($@"c:\temp\{ProjectionName}.cs", builder.ToString());
        }

        private void GenerateConstuctor(StringBuilder builder, Type[] eventTypes)
        {
            builder.AppendLine($@"       public {ProjectionName}()");
            builder.AppendLine(@"       {");
            foreach (var eventType in eventTypes)
            {
                builder.AppendLine($@"           ProjectEvent<{eventType.Name}>(Project);");
            }
            builder.AppendLine(@"       }");
            builder.AppendLine();
        }

        private void GenerateProjector(StringBuilder builder, Type eventType)
        {
            builder.AppendLine($@"       private void Project({ViewType} view, {eventType.Name} @event)");
            builder.AppendLine(@"       {");

            foreach (var propertyInfo in eventType.GetProperties())
            {
                builder.AppendLine($@"          view.{propertyInfo.Name} = @event.{propertyInfo.Name};");
            }

            builder.AppendLine($@"           view.LastUpdated = @event.EventTimestamp;");
            builder.AppendLine(@"       }");
            builder.AppendLine();
        }
    }
}
