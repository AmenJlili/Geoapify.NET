using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geoapify.NET
{
    public class AutocompleteHelper
    {

        public static Task<Address[]> GetAddressAsync(string address, string apiKey = "")
        {
            return Task.Run<Address[]>(() => { return GetAddress(address, apiKey); });
        }

            public static Address[] GetAddress(string address, string apiKey = "")
        {
            var client = new RestClient("https://api.geoapify.com/v1/geocode/autocomplete?filter=countrycode:us&text=" + address + "&apiKey=" + apiKey);
            client.Timeout = 10000;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

           

            var ret = JsonConvert.DeserializeObject<Root>(response.Content);

            if (ret == null)
                return new Address[] { };


            if (ret.features == null)
                return new Address[] { };

            return ret.features.Select(x=> x.Address).ToArray();
        }
    }

    public class Datasource
    {
        public string sourcename { get; set; }
        public string attribution { get; set; }
        public string license { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        [JsonProperty("properties")]
        public Address Address { get; set; }

       
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Parsed
    {
        public string housenumber { get; set; }
        public string street { get; set; }
        public string expected_type { get; set; }
    }

    public class Address
    {
        public string country_code { get; set; }
        public string housenumber { get; set; }
        public string street { get; set; }
        public string country { get; set; }
        public string county { get; set; }
        public Datasource datasource { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string suburb { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
        public string state_code { get; set; }
        public string formatted { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public Timezone timezone { get; set; }
        public string result_type { get; set; }
        public Rank rank { get; set; }
        public string place_id { get; set; }
        public string postcode { get; set; }


        public void Reset()
        {

        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(formatted) == false)
                return formatted;

            return base.ToString();
        }
    }

    public class Query
    {
        public string text { get; set; }
        public Parsed parsed { get; set; }
    }

    public class Rank
    {
        public double confidence { get; set; }
        public string match_type { get; set; }
    }

    public class Root
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
        public Query query { get; set; }
    }

    public class Timezone
    {
        public string name { get; set; }
        public string offset_STD { get; set; }
        public int offset_STD_seconds { get; set; }
        public string offset_DST { get; set; }
        public int offset_DST_seconds { get; set; }
        public string abbreviation_STD { get; set; }
        public string abbreviation_DST { get; set; }
    }

}

namespace Geoapify.NET.WPF
{ 

   
}