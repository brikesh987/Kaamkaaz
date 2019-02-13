namespace Kaamkaaz.Services
{
    using Kaamkaaz.Helpers;
    using Kaamkaaz.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xamarin.Forms.Internals;    

    /// <summary>
    /// Defines the <see cref="KaamkaazService" />
    /// </summary>
    public static class KaamkaazService
    {
        #region Methods

        /// <summary>
        /// The Broadcast
        /// </summary>
        /// <param name="message">The message<see cref="BroadcastMessage"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool Broadcast(BroadcastMessage message)
        {
            return true;
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
                Log.Warning("Servicces", $"Unexpected error getting service types: {ex.Message}");
            }

            //List<string> result = GetServiceAsync(country).ConfigureAwait(false).GetAwaiter().GetResult();
            return result;
        }

        /// <summary>
        /// The SaveProfile
        /// </summary>
        /// <param name="profile">The profile<see cref="Profile"/></param>
        public static void SaveProfile(Profile profile)
        {
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

        #endregion
    }
}
