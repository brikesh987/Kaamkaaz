namespace KaamkaazServices.Filters
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="HawkAuthenticationHandler" />
    /// </summary>
    public class HawkAuthenticationHandler : AuthenticationHandler<HawkAuthenticationOptions>
    {
        #region Constants

        /// <summary>
        /// Defines the AuthorizationHeaderName
        /// </summary>
        private const string AuthorizationHeaderName = "Authorization";

        /// <summary>
        /// Defines the SchemeName
        /// </summary>
        private const string SchemeName = "amx";
        private static Dictionary<string, string> allowedApps = new Dictionary<string, string>();
        private readonly UInt64 requestMaxAgeInSeconds = 300;//5 mins

        #endregion

        #region Constructors

        //private readonly IBasicAuthenticationService _authenticationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="HawkAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="IOptionsMonitor{HawkAuthenticationOptions}"/></param>
        /// <param name="logger">The logger<see cref="ILoggerFactory"/></param>
        /// <param name="encoder">The encoder<see cref="UrlEncoder"/></param>
        /// <param name="clock">The clock<see cref="ISystemClock"/></param>
        public HawkAuthenticationHandler(
            IOptionsMonitor<HawkAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            if (allowedApps.Count == 0)
            {
                allowedApps.Add("4edf032b-da82-4d7f-bd37-b0a058f32be7", "fUQVBTexG4PnXgsjqycciOYSQyWW94J1ZeQ4Q0Q3Qz4=");
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// The HandleAuthenticateAsync
        /// </summary>
        /// <returns>The <see cref="Task{AuthenticateResult}"/></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }
            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!SchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }
            var autherizationHeaderArray = GetAuthorizationHeaderValues(headerValue.Parameter);
            if (autherizationHeaderArray.Length != 4)
            {
                AuthenticateResult.Fail("Incomplete authentication parameters");
            }

            var APPId = autherizationHeaderArray[0];
            var incomingBase64Signature = autherizationHeaderArray[1];
            var nonce = autherizationHeaderArray[2];
            var requestTimeStamp = autherizationHeaderArray[3];


            var isValid = IsValidRequest(Request, APPId, incomingBase64Signature, nonce, requestTimeStamp);
            if (!isValid)
            {
                return AuthenticateResult.NoResult();
            }

            var hashValue = "";
            var claims = new[] { new Claim(ClaimTypes.Hash, hashValue) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        /// <summary>
        /// The HandleChallengeAsync
        /// </summary>
        /// <param name="properties">The properties<see cref="AuthenticationProperties"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }

        /// <summary>
        /// The ComputeHash
        /// </summary>
        /// <param name="httpContent">The httpContent<see cref="Stream"/></param>
        /// <returns>The <see cref="byte[]"/></returns>
        private static byte[] ComputeHash(System.IO.Stream httpContent)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                //var content = await httpContent.ReadToEndAsync();
                //if (content.Length != 0)
                //{
                hash = md5.ComputeHash(httpContent);
                return hash;
            }
        }

        /// <summary>
        /// The GetAuthorizationHeaderValues
        /// </summary>
        /// <param name="rawAuthzHeader">The rawAuthzHeader<see cref="string"/></param>
        /// <returns>The <see cref="string[]"/></returns>
        private string[] GetAuthorizationHeaderValues1(string rawAuthzHeader)
        {
            var listOfHawkParam = new List<string>();
            var credArray = rawAuthzHeader.Split(',');

            if (credArray.Length != 4)
            {
                return null;
            }
            else
            {
                foreach (var item in credArray)
                {
                    var cred = item.Split('=')[1].Replace("\"", "");

                    listOfHawkParam.Add(cred);
                }
            }
            return listOfHawkParam.ToArray<string>();
        }

        private string[] GetAuthorizationHeaderValues(string rawAuthzHeader)
        {

            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The isReplayRequest
        /// </summary>
        /// <param name="nonce">The nonce<see cref="string"/></param>
        /// <param name="requestTimeStamp">The requestTimeStamp<see cref="string"/></param>
        /// <param name="context">The context<see cref="AuthorizationFilterContext"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            var memoryCache = Request.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var value = "";
            if (memoryCache.TryGetValue(nonce, out value))
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
            {
                return true;
            }

            memoryCache.Set(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

            return false;
        }

        /// <summary>
        /// The IsValidRequest
        /// </summary>
        /// <param name="req">The req<see cref="HttpRequest"/></param>
        /// <param name="APPId">The APPId<see cref="string"/></param>
        /// <param name="incomingBase64Signature">The incomingBase64Signature<see cref="string"/></param>
        /// <param name="nonce">The nonce<see cref="string"/></param>
        /// <param name="requestTimeStamp">The requestTimeStamp<see cref="string"/></param>
        /// <param name="context">The context<see cref="AuthorizationFilterContext"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool IsValidRequest(HttpRequest req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            string requestContentBase64String = "";
            string requestUri = HttpUtility.UrlEncode(req.Path.Value.ToLower());
            string requestHttpMethod = req.Method;

            if (!allowedApps.ContainsKey(APPId))
            {
                return false;
            }

            var sharedKey = allowedApps[APPId];

            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            req.EnableRewind();
            byte[] hash = ComputeHash(req.Body);
            // Rewind, so the core is not lost when it looks the body for the request
            req.Body.Position = 0;

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }
        }

        #endregion
    }
}
