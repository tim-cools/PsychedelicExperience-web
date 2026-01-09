using System;
using System.IO;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Unit
{
    public class ExperienceAdded
    {
        public string A { get; set; }
        public UserId B { get; set; }
        public Title C { get; set; }
        public UserId D { get; set; }
        public string E { get; set; }
    }

    public class Event
    {
        public UserId UserId { get; set; }
        public UserId ExperienceId { get; set; }
        public Description Description { get; set; }
    }

    public class EventEncryptedString
    {
        public UserId UserId { get; set; }
        public UserId ExperienceId { get; set; }
        public EncryptedString Description { get; set; }
    }

    public class WhenConvertingADescriptionFromJson
    {
        [Fact]
        public void ThenTheResultShouldBeTheSame()
        {
            var value = new Description("halllooo");

            var json = Serialize(value);
            var description = Deserialize<Description>(json);

            description.ShouldNotBeNull();
            description.Value.ShouldBe("halllooo");
        }

        [Fact]
        public void GivenTheOriginalSerializerThenTheResultShouldBeTheSame()
        {
            var value = new Description("halllooo");

            var json = Serialize(value, false);
            var description = Deserialize<Description>(json);

            description.ShouldNotBeNull();
            description.Value.ShouldBe("halllooo");
        }

        [Fact]
        public void GivenTheOriginalSerializerWithNestedValueThenTheResultShouldBeTheSame()
        {
            var value = new ExperienceAdded
            {
                A = "A",
                 B = UserId.New(),
                 C = new Title("azertyuio4567890"),
                 D = UserId.New(),
                 E = "E"
            };

            var json = Serialize(value, false);
            var description = Deserialize< ExperienceAdded>(json);

            description.ShouldNotBeNull();
            description.A.ShouldBe("A");
            description.B.ShouldBe(value.B);
            description.C.Value.ShouldBe("azertyuio4567890");
            description.D.ShouldBe(value.D);
            description.E.ShouldBe("E");
        }

        [Fact]
        public void GivenARealExampleThenTheResultShouldBeTheSame()
        {
            var json = "{\"UserId\": \"bd13a9a6-0e43-4828-9564-a1b76917d9eb\", " +
                       "\"Description\": {\"Value\": \"test the notes\"}, " +
                       "\"ExperienceId\": \"0159a7cd-b511-4b45-8890-4b8f61d34968\", " +
                       "\"EventTimestamp\": \"2017-01-16T19:21:27.5115142+00:00\"}\"";

            var description = Deserialize<Event>(json);

            description.ShouldNotBeNull();

            description.ExperienceId.ShouldNotBeNull();
            description.ExperienceId.Value.ShouldBe(new Guid("0159a7cd-b511-4b45-8890-4b8f61d34968"));
        }

        [Fact]
        public void GivenARealExampleWithEncryptedStringThenTheResultShouldBeTheSame()
        {
            var json = "{\"UserId\": \"bd13a9a6-0e43-4828-9564-a1b76917d9eb\", " +
                       "\"Description\": {\"Value\": \"test the notes\"}, " +
                       "\"ExperienceId\": \"0159a7cd-b511-4b45-8890-4b8f61d34968\", " +
                       "\"EventTimestamp\": \"2017-01-16T19:21:27.5115142+00:00\"}\"";

            var description = Deserialize<EventEncryptedString>(json);

            description.ShouldNotBeNull();

            description.ExperienceId.ShouldNotBeNull();
            description.ExperienceId.Value.ShouldBe(new Guid("0159a7cd-b511-4b45-8890-4b8f61d34968"));
        }

        private static string Serialize<T>(T value, bool addConverter = true)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IddConverter<UserId>());
            if (addConverter)
            {
                serializer.Converters.Add(new EncryptedStringConverter());
                serializer.Converters.Add(new DescriptionConvertor());
            }

            string json;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, value);

                json = writer.ToString();
            }
            return json;
        }

        private static T Deserialize<T>(string json)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IddConverter<UserId>());
            serializer.Converters.Add(new EncryptedStringConverter());
            serializer.Converters.Add(new DescriptionConvertor());

            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
