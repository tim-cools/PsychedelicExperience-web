using System.IO;
using System.Runtime.InteropServices;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = args.Length > 0 
                ? args[1] 
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) 
                    ? @"/Users/timcools/_/pex/git/src/PsychedelicExperience.Psychedelics.Messages/"
                    : @"C:\_\psychedelicexperience\git\src";

            Generate(basePath, new Generator());
        }

        private static void Generate(string basePath, IGenerator generator)
        {
            var files = GetFiles(basePath, generator.Extension);
            foreach (var file in files)
            {
                var content = File.ReadAllLines(file);
                var result = generator.Process(content);

                File.WriteAllText($"{file}.cs", result);
            }
        }

        private static string[] GetFiles(string basePath, string extension)
        {
            return Directory.GetFiles(basePath, $"*.{extension}", SearchOption.AllDirectories);
        }
    }
}
