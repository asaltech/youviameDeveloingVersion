using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using youviame.API.Controllers;
using youviame.Data.Enitities;
using youviame.Data.Repositories;

namespace youviame.API.UserContext {
    [RoutePrefix("Account")]
    public class AccountController : ApiController {
        private readonly IUserRepository _userRepository;
        private AuthRepository _repo = null;

        public AccountController(IUserRepository userRepository) {
            _userRepository = userRepository;
            _repo = new AuthRepository();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("LoginExternal")]
        public async Task<IHttpActionResult> LoginExternal(RegisterExternalBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null) {
                return BadRequest("Invalid Provider or External Access Token");
            }
            IdentityUser user = null;
            try {
                user = await _repo.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            
            UserViewModel userViewModel;
            var hasRegistered = user != null;
            if (hasRegistered) {
                var existingUser = _userRepository.Get(verifiedAccessToken.user_id) ??
                                   await CreateProfile(model.Provider, model.ExternalAccessToken);
                userViewModel = existingUser.ToViewModel();
            }
            else {
                user = new IdentityUser { UserName = model.UserName };
                var result = await _repo.CreateAsync(user);
                if (!result.Succeeded) {
                    return GetErrorResult(result);
                }

                var info = new ExternalLoginInfo {
                    DefaultUserName = model.UserName,
                    Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
                };

                result = await _repo.AddLoginAsync(user.Id, info.Login);

                if (!result.Succeeded) {
                    return GetErrorResult(result);
                }
                var youviameuser = await CreateProfile(model.Provider, model.ExternalAccessToken);
                await _userRepository.SaveAsync(youviameuser);
                userViewModel = youviameuser.ToViewModel();
            }
            var accessTokenResponse = GenerateLocalAccessTokenResponse(model.UserName, model.ExternalAccessToken);
            var response = new LoginExternalUserResponse {
                AccessToken = accessTokenResponse,
                User = userViewModel
            };
            var statusCode = hasRegistered ? HttpStatusCode.OK : HttpStatusCode.Created;
            var httpResponseMessage = Request.CreateResponse(statusCode, response);
            return ResponseMessage(httpResponseMessage);
        }

        public class LoginExternalUserResponse {
            public JObject AccessToken { get; set; }
            public UserViewModel User { get; set; }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _repo.Dispose();
            }
            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result) {
            if (result == null) {
                return InternalServerError();
            }

            if (!result.Succeeded) {
                if (result.Errors != null) {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid) {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }
            return null;
        }

        
        private async Task<User> CreateProfile(string provider, string accessToken) {
            User user = null;
            if (provider != "Facebook")
                return null;
            user = new User();
            user.DateModeEnabled = true;
            var facebookClient = new FacebookClient(accessToken) { Version = "v2.8" };
            var facebookUserData = await facebookClient.GetTaskAsync("me?fields=id,first_name,last_name,albums.limit(10){id,type},about,location,picture.width(300).height(300)");
            var facebookProfileJsonString = JsonConvert.SerializeObject(facebookUserData);
            var facebookProfile = JsonConvert.DeserializeObject<FacebookProfile>(facebookProfileJsonString);
            user.FacebookId = facebookProfile.Id;
            //  user.SetLocation(facebookProfile.Location?.Name ?? string.Empty);
            user.SetLocation(facebookProfile.Location !=null ?facebookProfile.Location.Name : string.Empty);
            user.FirstName = facebookProfile.FirstName;
            user.LastName = facebookProfile.LastName;
            //  user.SetAboutMe(facebookProfile.About ?? $"{facebookProfile.FirstName} {facebookProfile.LastName}");
            user.SetAboutMe(facebookProfile.About ?? String.Format("{0} {1}", facebookProfile.FirstName, facebookProfile.LastName));
            var images = new List<Image>();
            //  var profileAlbum = facebookProfile.Albums?.Data?.FirstOrDefault(x => x.Type == "profile");
            //if
            var Album = facebookProfile.Albums;
            var AlbumData = Album != null ? Album.Data : null;
            var profileAlbum = AlbumData != null ? facebookProfile.Albums.Data.FirstOrDefault(x => x.Type == "profile") :null;
            if (profileAlbum != null) {
                //var profilePicturesData = await facebookClient.GetTaskAsync($"{profileAlbum.Id}/photos?fields=picture.width(300).height(300)");
                var profilePicturesData = await facebookClient.GetTaskAsync(String.Format("{0}/photos?fields=picture.width(300).height(300)", profileAlbum.Id));
                var profilePicturesJsonString = JsonConvert.SerializeObject(profilePicturesData);
                var profilePictures = JsonConvert.DeserializeObject<AlbumPictures>(profilePicturesJsonString);
                foreach (var profilePicture in profilePictures.Data) {
                    var image = new Image {
                        Id = profilePicture.Id,
                        Url = profilePicture.Picture
                    };
                    images.Add(image);
                }
            }

            if (images.Count == 0) {
                var profilePic = new Image {
                    Id = "profilePicture",
                    Url = facebookProfile.Picture.Data.Url
                };
                images.Add(profilePic);
            }
            user.SetProfilePictures(images.Take(4).ToList());
            await _userRepository.SaveAsync(user);
            return user;
        }



        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken) {
            ParsedExternalAccessToken parsedToken = null;
            var verifyTokenEndpoint = "";
            if (provider == "Facebook") {
                var appToken = "1685342265085833|05ezSwap0JqcKSavvnVGVmx3QGM";
                // verifyTokenEndpoint =$"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appToken}";

                verifyTokenEndpoint = String.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}",accessToken,appToken);
             
                    
            }
            else {
                return null;
            }
            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndpoint);
            try {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode) {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic jObj = (JObject)JsonConvert.DeserializeObject(content);

                    parsedToken = new ParsedExternalAccessToken();

                    if (provider == "Facebook") {
                        parsedToken.user_id = jObj["data"]["user_id"];
                        parsedToken.app_id = jObj["data"]["app_id"];

                        if (
                            !string.Equals(Startup.FacebookAuthOptions.AppId, parsedToken.app_id,
                                StringComparison.OrdinalIgnoreCase)) {
                            return null;
                        }
                    }
                }
                return parsedToken;
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

        }

        private JObject GenerateLocalAccessTokenResponse(string userName, string token) {
            var tokenExpiration = TimeSpan.FromDays(1);
            var identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("ExternalAccessToken", token, ClaimValueTypes.String, "Facebook", "Facebook"));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration)
            };

            var ticket = new AuthenticationTicket(identity, props);
            try {
                var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
                var tokenResponse = new JObject {
                new JProperty("userName", userName),
                new JProperty("access_token", accessToken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                new JProperty("issued", ticket.Properties.IssuedUtc.ToString()),
                new JProperty("expires", ticket.Properties.ExpiresUtc.ToString())
            };
                return tokenResponse;
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}