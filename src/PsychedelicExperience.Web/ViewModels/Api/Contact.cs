
namespace PsychedelicExperience.Web.ViewModels.Api
{
    public class Contact
    {
        /// <summary>
        /// The type of contact: 'website', 'phone' or 'email'.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The value of the contact.
        /// </summary>
        public string Value { get; set; }
    }
}