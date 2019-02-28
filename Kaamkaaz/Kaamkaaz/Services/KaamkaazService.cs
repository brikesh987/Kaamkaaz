namespace Kaamkaaz.Services
{
    using Kaamkaaz.Helpers;
    using Kaamkaaz.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Xamarin.Forms.Internals;

    /// <summary>
    /// Defines the <see cref="KaamkaazService" />
    /// </summary>
    public static class KaamkaazService
    {
        #region Methods
        public static List<ServiceProvider> GetServiceProviders(Location curLocation, int userId)
        {
            var providers = new List<ServiceProvider>();
            try
            {
                providers = AsyncHelper.RunSync<List<ServiceProvider>>(() => GetServicePrividersAsync(curLocation,userId));
            } catch(Exception ex)
            {
                Log.Warning("Servicces", $"Unexpected error getting service providers: {ex.Message}");
            }
            return providers;
        }

        private static async Task<List<ServiceProvider>> GetServicePrividersAsync(Location curLocation,int userId)
        {
            var list = new List<ServiceProvider>();
            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            client.DefaultRequestHeaders.ExpectContinue = false;
            
            HttpResponseMessage response = await client.GetAsync($"{AppConstants.AppBaseAddress}/api/SerivceProvider?Latitude={curLocation.Latitude}&Longitude={curLocation.Longitude}&City={curLocation.City}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                list = JsonConvert.DeserializeObject<List<ServiceProvider>>(responseString);
            }
            return list;
        }

        /// <summary>
        /// The Broadcast
        /// </summary>
        /// <param name="message">The message<see cref="BroadcastMessage"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool Broadcast(BroadcastMessage message)
        {
            bool result = false;
            try
            {
                result = AsyncHelper.RunSync<bool>(() => SendMessage(message));
            }
            catch (Exception ex)
            {
                Log.Warning("Service", $"Unexpected error sending message for broadcast: {ex.Message}");
            }

            //List<string> result = GetServiceAsync(country).ConfigureAwait(false).GetAwaiter().GetResult();
            return result;
        }

        public static Profile GetUser(int userId)
        {
            var user = new Profile();
            try
            {
                user = AsyncHelper.RunSync<Profile>(() => GetUserAsync(userId));
            }
            catch (Exception ex)
            {
                Log.Warning("Service", $"Unexpected error getting user information: {ex.Message}");
            }
            return user;
        }

        private static async Task<Profile> GetUserAsync(int userId)
        {
            var user = new Profile();
            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            client.DefaultRequestHeaders.ExpectContinue = false;
            HttpResponseMessage response = await client.GetAsync($"{AppConstants.AppBaseAddress}/api/Users?id={userId}");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                user = JsonConvert.DeserializeObject<Profile>(responseString);
            }
            return user;
        }

        /// <summary>
        /// The GetSericeTypes
        /// </summary>
        /// <param name="country">The country<see cref="string"/></param>
        /// <returns>The <see cref="List{string}"/></returns>
        public static List<string> GetSericeTypes(string country)
        {
            List<string> result = new List<string>();
            try
            {
                result = AsyncHelper.RunSync<List<string>>(() => GetServiceAsync(country));
            }
            catch (Exception ex)
            {
                Log.Warning("Service", $"Unexpected error getting service types: {ex.Message}");
            }

            //List<string> result = GetServiceAsync(country).ConfigureAwait(false).GetAwaiter().GetResult();
            return result;
        }

        /// <summary>
        /// The SaveProfile
        /// </summary>
        /// <param name="profile">The profile<see cref="Profile"/></param>
        public static bool SaveProfile(Profile profile)
        {
            var result = false;
            try
            {
                result = AsyncHelper.RunSync<bool>(() => SaveProfileAsync(profile));
            }
            catch (Exception ex)
            {
                Log.Warning("Service", $"Unexpected error sending saving user profile: {ex.Message}");
            }
            return result;
        }

        private static async Task<bool> SaveProfileAsync(Profile profile)
        {
            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);

            string postBody = JsonConvert.SerializeObject(profile);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PutAsJsonAsync<Profile>(AppConstants.AppBaseAddress + "/api/Users", profile); ;

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                var serverResult = JsonConvert.DeserializeObject<UserResponse>(responseString);
                if (serverResult.Success)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// The GetServiceAsync
        /// </summary>
        /// <param name="country">The country<see cref="string"/></param>
        /// <returns>The <see cref="Task{List{string}}"/></returns>
        private static async Task<List<string>> GetServiceAsync(string country)
        {
            var list = new List<string>();
            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);
            client.DefaultRequestHeaders.ExpectContinue = false;
            HttpResponseMessage response = await client.GetAsync($"{AppConstants.AppBaseAddress}/api/Services?country={country}");

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                list = JsonConvert.DeserializeObject<List<string>>(responseString);
            }
            return list;
        }

        /// <summary>
        /// The SendMessage
        /// </summary>
        /// <param name="message">The message<see cref="BroadcastMessage"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        private static async Task<bool> SendMessage(BroadcastMessage message)
        {

            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient client = HttpClientFactory.Create(customDelegatingHandler);

            string postBody = JsonConvert.SerializeObject(message);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync<BroadcastMessage>(AppConstants.AppBaseAddress + "/api/Messages", message); ;

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                return true;
            }
            return false;
        }

        #endregion
    }
}
