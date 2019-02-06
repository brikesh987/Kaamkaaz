using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppKeyGenarator
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32]; //256 bit
                cryptoProvider.GetBytes(secretKeyByteArray);
                var APIKey = Convert.ToBase64String(secretKeyByteArray);
                Console.WriteLine(APIKey);
                Console.WriteLine(Guid.NewGuid().ToString());
            }
            Console.Read();
            */
            //RunAsync().Wait();
           PostAsync().Wait();
        }
        static async Task RunAsync()
        {
            Console.WriteLine("Calling the back-end API");

            string apiBaseAddress = "https://localhost:44343";

            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            var country = "India";
            HttpResponseMessage response = await client.GetAsync($"{apiBaseAddress}/api/Services?country={country}");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
                Console.WriteLine("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                Console.WriteLine("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
            }

            Console.ReadLine();
        }

        static async Task PostAsync()
        {
            Console.WriteLine("Calling the back-end API");

            string apiBaseAddress = "https://localhost:44343/";

            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            var user = new User() { Name = "TestUser", Phone = "4324343", ProfileData = new List<string>() { "Barber" },IsActive = true,AboutUser="Test",
                Location = new Location() { Longitude = 12.220011, Latitude = 12.330022, City = "Bokaro", Country = "India" } };

            string postBody = JsonConvert.SerializeObject(user);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync<User>(apiBaseAddress + "api/Users", user); ;
            //HttpResponseMessage response = await client.PostAsJsonAsync(apiBaseAddress + "api/Users", ""); 

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
                Console.WriteLine("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                Console.WriteLine("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
            }

            Console.ReadLine();
        }
    }

    public class User
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string AboutUser { get; set; }
        public Location Location { get; set; }
        public List<string> ProfileData { get; set; }
    }

    public class Location
    {
         /// <summary>
        /// Gets or sets the City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude
        /// </summary>
        public double Longitude { get; set; }

        
    }
  }

