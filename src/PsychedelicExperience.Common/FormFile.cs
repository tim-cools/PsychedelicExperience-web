using System;

namespace PsychedelicExperience.Common
{
    public class FormFile
    {
        public Guid Id { get; }
        public string FileName { get; }
        public string OriginalFileName { get; }

        public FormFile(Guid id, string fileName, string originalFileName)
        {
            Id = id;
            FileName = fileName;
            OriginalFileName = originalFileName;
        }
    }
}