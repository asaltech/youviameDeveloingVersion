using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace youviame.API.Controllers {
    public class ResultWithChallenge : IHttpActionResult {
        private readonly string authenticationScheme = "amx";
        private readonly IHttpActionResult _next;

        public ResultWithChallenge(IHttpActionResult next) {
            this._next = next;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            var response = await _next.ExecuteAsync(cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(authenticationScheme));
            }
            return response;
        }
    }
}