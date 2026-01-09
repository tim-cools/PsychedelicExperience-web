//using System.Collections.Generic;
//using PsychedelicExperience.Psychedelics.Messages.Organisations;

//namespace PsychedelicExperience.Web.ViewModels.Api
//{
//    public class GooglePlaceAddress
//    {
//        public address_component[] address_components { get; set; }
//        public string formatted_address { get; set; }
//        public string formatted_phone_number { get; set; }
//        public geometry geometry { get; set; }

//        public Address ToAddress()
//        {
//            return new Address(Name, Attributes);
//        }
//    }

//    public class geometry
//    {
//        public location location { get; set; }
//        public location location { get; set; }
//    }

//    public class location
//    {
//        private string latitude { get; set; }
//        private string longitude { get; set; }
//    }

//    public class address_component
//    {
//        public string[] types {get; set; }
//        public string long_name { get; set; }
//        public string short_name { get; set; }
//    }
//}