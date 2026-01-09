using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Research
{
    public class GenerateCommandHandlerTestTemplate
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
                .ToArray();
            return commandTypes;
        }

        private void GenerateTests(Type[] commandTypes)
        {
            var builder = new StringBuilder();
            foreach (var commandType in commandTypes)
            {
                var aggregate = GetAggregateName(commandType);
                var aggregateId = GetAggregateId(commandType);
                GenerateValidator(builder, commandType);
                GenerateHandler(builder, commandType, aggregate, aggregateId);
            }

            File.WriteAllText(@"c:\temp\commandHandlers.cs", builder.ToString());
        }

        private string GetAggregateId(Type commandType)
        {
            return "DocumentId";
        }

        private string GetAggregateName(Type commandType)
        {
            return "Document";
        }

        private void GenerateValidator(StringBuilder builder, Type commandType)
        {
            builder.AppendLine($@"      public class {commandType.Name}Validator : AbstractValidator<{commandType.Name}>");
            builder.AppendLine(@"    {");
            builder.AppendLine($@"        public {commandType.Name}Validator()");
            builder.AppendLine(@"        {");

            foreach (var propertyInfo in commandType.GetProperties())
            {
                builder.AppendLine($@"            RuleFor(command => command.{propertyInfo.Name}).{propertyInfo.Name}();");
            }

            builder.AppendLine(@"        }");
            builder.AppendLine(@"    }");
            builder.AppendLine();
        }

        private void GenerateHandler(StringBuilder builder, Type commandType, string aggregateName, string aggregateId)
        {
            builder.AppendLine($@"      public class {commandType.Name}Handler : AggregateCommandHandler<{commandType.Name}, {aggregateName}, {aggregateId}>");
            builder.AppendLine(@"    {");
            builder.AppendLine($@"        public {commandType.Name}Handler(IDocumentSession session, IValidator<{commandType.Name}> commandValidator) :");
            builder.AppendLine($@"            base(session, commandValidator,");
            builder.AppendLine($@"                command => command.{aggregateId},");
            builder.AppendLine($@"                command => command.UserId,");
            builder.AppendLine($@"                (aggregate, user) => aggregate.EnsureCanEdit(user))");
            builder.AppendLine(@"        {");
            builder.AppendLine(@"        }");
            builder.AppendLine(@"    }");
            builder.AppendLine();
        }
    }
}
