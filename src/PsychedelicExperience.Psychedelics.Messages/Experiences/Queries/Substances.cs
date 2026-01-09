namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class Substances
    {
        public static string[] All = {"Psilocybe", "MDMA", "LSD", "Mescaline", "Ayahuasca", "Iboga", "Salvia", "DMT"};

        public static string LSD = "LSD";
        public static string DMT = "DMT";

        public static string Get(int index)
        {
            return All[index % All.Length];
        }
    }
}