using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace TheWorld.Services
{
    public class GeoCoordsService
    {
        private ILogger<GeoCoordsService> _logger;
        private IConfigurationRoot _config;

        // Constructor. at minimum, need a ILogger
        public GeoCoordsService(ILogger<GeoCoordsService> logger, IConfigurationRoot config )
        {
            _logger = logger;
            _config = config;
        }

        public async Task<GeoCoordsResult> GetCoordsAsync(string name)  // generate GeoCoordResult in new file. move from root to Services dir
        {
            // make an instance of the result class and set some default values in case of a failure
            // when successful, just change the props
            var result = new GeoCoordsResult()
            {
                Success = false,
                Message = "Failed to get coordinates"
            };

            // Get Bing Maps service to convert names to long/lat (www.bingmapsportal.com)
            var apiKey = _config["Keys:BingKey"];    // in Window Enviro Vari it is a double under score but can still use colon in config object
            var encodedName = WebUtility.UrlEncode(name); // need because going to generate this as an url as a uri

            // Build a URL ( a service address) that will get our coordinbates for us based on name we pass in 
            // from Bing dev. Takes 2 params: 1 is our encodedName and 2 is a key, our apiKey
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedName}&key={apiKey}";

            // This comes the resources folder of the project demo files

            // Create a client for use
            var client = new HttpClient();

            // Calls GetStringAsync(url) on the url to get th eresults of this query for the name
            var json = await client.GetStringAsync(url);  // return value is in json

            // Read out the results
            // Fragile, might need to change if the Bing API changes
            var results = JObject.Parse(json);  //parse with .Net json parser.  Allows us to walk through it and interogate it
            var resources = results["resourceSets"][0]["resources"];  // look for the resources we searched for
            if (!results["resourceSets"][0]["resources"].HasValues)  // if failed to return
            {
                result.Message = $"Could not find '{name}' as a location";
            }
            else  // if returned do a test with a prop called confidence. Determines how sure it is that the lat/long are correct. 
            // if low we choose not to trust it 
            {
                var confidence = (string)resources[0]["confidence"];
                if (confidence != "High")
                {
                    result.Message = $"Could not find a confident match for '{name}' as a location";
                }
                else  // if found and confidence is high then set coords (a JToken) 
                {
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    // set the result properties
                    result.Latitude = (double) coords[0];
                    result.Longitude = (double) coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
            }
            return result;
        }
    }
}
