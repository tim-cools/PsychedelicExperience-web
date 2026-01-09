using System.ComponentModel.DataAnnotations;

namespace PsychedelicExperience.ViewModels
{
    public class UserModel
    {
        public string FullName { get; set; }
        public string DisplayName { get; set; }

        public string EMail { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}