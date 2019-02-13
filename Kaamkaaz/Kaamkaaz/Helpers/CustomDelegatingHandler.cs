using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kaamkaaz.Helpers
{
    /// <summary>
    /// Defines the <see cref="CustomDelegatingHandler" />
    /// </summary>
    public class CustomDelegatingHandler : DelegatingHandler
    {
        #region Fields

        /// <summary>
        /// Defines the APIKey
        /// </summary>
        private string APIKey = "fUQVBTexG4PnXgsjqycciOYSQyWW94J1ZeQ4Q0Q3Qz4=";

        //Obtained from the server earlier, APIKey MUST be stored securely and in App.Config
        /// <summary>
        /// Defines the APPId
        /// </summary>
        private string APPId = "4edf032b-da82-4d7f-bd37-b0a058f32be7";

        #endregion

        #region Methods

        /// <summary>
        /// The SendAsync
        /// </summary>
        /// <param name="request">The request<see cref="HttpRequestMessage"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{HttpResponseMessage}"/></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            HttpResponseMessage response = null;
            string requestContentBase64String = string.Empty;

            string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsolutePath.ToLower());

            string requestHttpMethod = request.Method.Method;

            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (request.Content != null)
            {
                byte[] content = await request.Content.ReadAsByteArrayAsync();
                MD5 md5 = MD5.Create();
                //Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            //Creating the raw signature string
            string signatureRawData = String.Format("{0}{1}{2}{3}{4}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce);

            var secretKeyByteArray = Convert.FromBase64String(APIKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                //Setting the values in the Authorization header using custom scheme (amx)
                request.Headers.Authorization = new AuthenticationHeaderValue("amx", string.Format("{0}:{1}:{2}:{3}", APPId, requestSignatureBase64String, nonce, requestTimeStamp));
            }

            response = await base.SendAsync(request, cancellationToken);

            return response;
        }

        #endregion
    }
}
