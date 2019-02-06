namespace KaamkaazServices.Filters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="HMacAuthorizationAttribute" />
    /// </summary>
    public class HMacAuthorizationAttribute : TypeFilterAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HMacAuthorizationAttribute"/> class.
        /// </summary>
        /// <param name="claimType">The claimType<see cref="string"/></param>
        /// <param name="claimValue">The claimValue<see cref="string"/></param>
        public HMacAuthorizationAttribute(string claimType, string claimValue) : base(typeof(HMACAuthorizationFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="HMACAuthorizationFilter" />
    /// </summary>
    public class HMACAuthorizationFilter : AuthorizeFilter //Attribute, IAuthorizationFilter
    {
        #region Fields

        /// <summary>
        /// Defines the authenticationScheme
        /// </summary>
        private readonly string authenticationScheme = "amx";

        /// <summary>
        /// Defines the requestMaxAgeInSeconds
        /// </summary>
        private readonly UInt64 requestMaxAgeInSeconds = 300;//5 mins

        /// <summary>
        /// Defines the allowedApps
        /// </summary>
        private static Dictionary<string, string> allowedApps = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HMACAuthorizationFilter"/> class.
        /// </summary>
        public HMACAuthorizationFilter()
        {
            if (allowedApps.Count == 0)
            {
                allowedApps.Add("4edf032b-da82-4d7f-bd37-b0a058f32be7", "fUQVBTexG4PnXgsjqycciOYSQyWW94J1ZeQ4Q0Q3Qz4=");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether AllowMultiple
        /// </summary>
        public bool AllowMultiple
        {
            get { return false; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The OnAuthorizationAsync
        /// </summary>
        /// <param name="context">The context<see cref="AuthorizationFilterContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var req = context.HttpContext.Request;

            if (req.Headers.ContainsKey("Authorization"))
            {
                var rawAuthzHeader = req.Headers["Authorization"];

                var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

                if (autherizationHeaderArray != null)
                {
                    var APPId = autherizationHeaderArray[0];
                    var incomingBase64Signature = autherizationHeaderArray[1];
                    var nonce = autherizationHeaderArray[2];
                    var requestTimeStamp = autherizationHeaderArray[3];

                    var isValid = IsValidRequest(req, APPId, incomingBase64Signature, nonce, requestTimeStamp, context);

                    if (isValid)
                    {
                        var currentPrincipal = new System.Security.Claims.ClaimsIdentity(new GenericIdentity(APPId));
                        context.HttpContext.User.AddIdentity(currentPrincipal);
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                }
                else
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// The ComputeHash
        /// </summary>
        /// <param name="httpContent">The httpContent<see cref="Stream"/></param>
        /// <returns>The <see cref="byte[]"/></returns>
        private static byte[] ComputeHash(Stream httpContent)
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
        /// The ChallengeAsync
        /// </summary>
        /// <param name="context">The context<see cref="HttpAuthenticationChallengeContext"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        //public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        //{
        //    context.Result = new ResultWithChallenge(context.Result);
        //    return Task.FromResult(0);
        //}

        /// <summary>
        /// The GetAutherizationHeaderValues
        /// </summary>
        /// <param name="rawAuthzHeader">The rawAuthzHeader<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
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
        private bool isReplayRequest(string nonce, string requestTimeStamp, AuthorizationFilterContext context)
        {
            var memoryCache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
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
        private bool IsValidRequest(HttpRequest req, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp, AuthorizationFilterContext context)
        {
            string requestContentBase64String = "";
            string requestUri = HttpUtility.UrlEncode(req.Path.Value.ToLower());
            string requestHttpMethod = req.Method;

            if (!allowedApps.ContainsKey(APPId))
            {
                return false;
            }

            var sharedKey = allowedApps[APPId];

            if (isReplayRequest(nonce, requestTimeStamp, context))
            {
                return false;
            }

            byte[] hash = ComputeHash(req.Body);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

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
