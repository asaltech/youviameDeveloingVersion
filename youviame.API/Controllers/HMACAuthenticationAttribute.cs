using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace youviame.API.Controllers {
    public class HMACAuthenticationAttribute : Attribute, IAuthenticationFilter {
        private static Dictionary<string, string> allowedApps = new Dictionary<string, string>();
        private readonly UInt64 requestMaxAgeInSeconds = 300;
        private readonly string authenticationScheme = "amx";

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken) {            
            allowedApps.Clear();
            allowedApps.Add("AppId", "vwDOWK2HJwXbO9l5IJQr5cRdz8K7Hxr435y5hDRSn/w=");
            var req = context.Request;
            if (req.Headers.Authorization != null &&
                authenticationScheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)) {
                var rawAuthHeader = req.Headers.Authorization.Parameter;
                var authorizationHeaderArray = GetAuthorizationHeaderValues(rawAuthHeader);
                if (authorizationHeaderArray != null) {
                    var appId = authorizationHeaderArray[0];
                    var incomingBase64Signature = authorizationHeaderArray[1];
                    var isValid = IsValidRequest(req, appId, incomingBase64Signature);
                    if (isValid.Result) {
                        var currentPrincipal = new GenericPrincipal(new GenericIdentity(appId), null);
                        context.Principal = currentPrincipal;
                    }
                    else {
                        context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    }
                }
                else {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            else {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }
            return Task.FromResult(0);
        }
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken) {
            context.Result = new ResultWithChallenge(context.Result);
            return Task.FromResult(0);
        }
        //  public bool AllowMultiple => false;
        public bool AllowMultiple = false;
        private static string[] GetAuthorizationHeaderValues(string rawAuthHeader) {
            var strings = rawAuthHeader.Split(':');
            if (strings.Length == 2)
                return strings;
            return null;
        }
        private async Task<bool> IsValidRequest(HttpRequestMessage req, string appId, string incomingBase64Signature) {
            var requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            var requestHttpMethod = req.Method.Method;

            if (!allowedApps.ContainsKey(appId))
                return false;

            var sharedKey = allowedApps[appId];
            // var data = $"{appId}{requestHttpMethod}{requestUri}";
            var data = String.Format("{0}{1}{2}",appId,requestHttpMethod,requestUri);
            var secretKeyBytes = Convert.FromBase64String(sharedKey);
            var signature = Encoding.UTF8.GetBytes(data);
            
            using (var hmac = new HMACSHA256(secretKeyBytes)) {
                var signatureBytes = hmac.ComputeHash(signature);
                var signatureBase64String = Convert.ToBase64String(signatureBytes);
                return
                    (incomingBase64Signature.Equals(signatureBase64String, StringComparison.Ordinal));
            }
        }

        bool IFilter.AllowMultiple
        {
            get { throw new NotImplementedException(); }
        }
    }
}