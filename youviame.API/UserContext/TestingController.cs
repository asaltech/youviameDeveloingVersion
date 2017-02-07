using System.Web.Http;

namespace youviame.API.UserContext {
    [RoutePrefix("testing")]
    public class TestingController : ApiController {
        [Authorize]
        [Route("")]
        public IHttpActionResult Get() {
            return Ok("You are authorized");
        }

    }
}