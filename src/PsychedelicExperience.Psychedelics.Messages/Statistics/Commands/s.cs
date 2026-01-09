using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Psychedelics.Messages.Statistics.Commands
{
    public enum Category
    {
        Users,
        All
    }

    public class GetStatistics : IRequest<Statistics>
    {
        public Format Format { get; }
        public Category Category { get; }

        public GetStatistics(Format format, Category category)
        {
            Format = format;
            Category = category;
        }
    }
}