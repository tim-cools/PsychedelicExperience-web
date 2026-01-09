using System;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    public static class RequestExtensions
    {
        public static void AddAddress(this RestRequest request)
        {
            request.AddParameter("address[formatted_address]", "somewhere");
            request.AddParameter("address[geometry][location]", "1,2");
            request.AddParameter("address[address_components][0][short_name]", "BE");
            request.AddParameter("address[address_components][0][long_name]", "BE");
            request.AddParameter("address[address_components][0][types][]", "country");

            //address[address_components][0][long_name]:Ayodele Fanoki Street
            //address[address_components][0][short_name]:Ayodele Fanoki St
            //address[address_components][0][types][]:route
            //address[address_components][1][long_name]:Lagos
            //address[address_components][1][short_name]:Lagos
            //address[address_components][1][types][]:colloquial_area
            //address[address_components][1][types][]:locality
            //address[address_components][1][types][]:political
            //address[address_components][2][long_name]:Lagos
            //address[address_components][2][short_name]:Lagos
            //address[address_components][2][types][]:administrative_area_level_1
            //address[address_components][2][types][]:political
            //address[address_components][3][long_name]:Nigeria
            //address[address_components][3][short_name]:NG
            //address[address_components][3][types][]:country
            //address[address_components][3][types][]:political
            //address[adr_address]:<span class="street-address">Ayodele Fanoki St</span>, <span class="locality">Lagos</span>, <span class="country-name">Nigeria</span>
            //address[formatted_address]:Ayodele Fanoki St, Lagos, Nigeria
            //address[geometry][location]:(6.6332023, 3.383400100000017)
            //address[geometry][viewport]:((6.63314315, 3.3833951999999954), (6.633268550000001, 3.38340560000006))
            //address[icon]:https://maps.gstatic.com/mapfiles/place_api/icons/doctor-71.png
            //address[id]:791dd46770b57651e2e95130bb9450f7770bf84b
            //address[name]:sdsdsd
            //address[place_id]:ChIJNSfqjnSTOxARfXugRn2AvZQ
            //address[reference]:CmRSAAAAuuU8ivLiRaAHTEMCvwBzopeK2IR3ewkdV5iZnaNnMDC5zqf0OB2hq1MzNcb0-o8q91Pt0wIjdUAuTY61aKh5W1WlYYHSvpWguPAxC8Uie_DTRj9gnRMABlZuhwc6mR_mEhDFMDUH8xHJ2kSJQYSEuAbCGhTP0gmvi-e27RBEGP48w64Qj2oVZA
            //address[reviews][0][aspects][0][rating]:0
            //address[reviews][0][aspects][0][type]:overall
            //address[reviews][0][author_name]:Busolami Adesany Biyaosi
            //address[reviews][0][author_url]:https://www.google.com/maps/contrib/100586840722693784512/reviews
            //address[reviews][0][language]:en
            //address[reviews][0][rating]:2
            //address[reviews][0][relative_time_description]:9 months ago
            //address[reviews][0][text]:
            //address[reviews][0][time]:1458412013
            //address[scope]:GOOGLE
            //address[types][]:hospital
            //address[types][]:point_of_interest
            //address[types][]:establishment
            //address[url]:https://maps.google.com/?cid=10717863963755838333
            //address[utc_offset]:60
            //address[vicinity]:Ayodele Fanoki Street
            //address[photos]:

        }

        public static void Add(this RestRequest request, Center center)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (center != null)
            {
                request.AddParameter("center.groupSize.maximum", center.GroupSize.Maximum);
            }
        }

        public static void Add(this RestRequest request, CenterReview center)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (center != null)
            {
                request.AddParameter("center.location.value", center.Location.Value);
                request.AddParameter("center.location.description", center.Location.Description);
                request.AddParameter("center.accommodation.value", center.Accommodation.Value);
                request.AddParameter("center.accommodation.description", center.Accommodation.Description);
                request.AddParameter("center.facilitators.value", center.Facilitators.Value);
                request.AddParameter("center.facilitators.description", center.Facilitators.Description);
                request.AddParameter("center.medicine.value", center.Medicine.Value);
                request.AddParameter("center.medicine.description", center.Medicine.Description);
                request.AddParameter("center.honest.value", center.Honest.Value);
                request.AddParameter("center.honest.description", center.Honest.Description);
            }
        }
    }
}