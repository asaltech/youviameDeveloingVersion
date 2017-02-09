using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using Facebook;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using youviame.Data.Enitities;
using youviame.Data.Repositories;

namespace youviame.API.Controllers {
    //[Authorize]
    [RoutePrefix("user")]
    public class UserController : BaseApiController {

        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        public UserController(IUserRepository userRepository, ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        [Route("update")]
        [HttpPut]
        public async Task<HttpResponseMessage> UpdateProfile([FromBody] UpdateProfileRequest request) {
            _logRepository.InsertLog("Update Profile requested");
        
            var user = _userRepository.Get(request.UserId);
            if (user == null)
            {
                _logRepository.InsertLog("User does not exist");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User does not exist");
            }
            _logRepository.InsertLog("before oldvalues");
            var oldValues = new EditProfileModel {
                DateModeEnabled = user.DateModeEnabled,
                Location = user.Location,
                ProfilePictures = JsonConvert.DeserializeObject<List<Image>>(user.ProfilePictures),
                AboutMe = user.AboutMe
            };
            _logRepository.InsertLog("after oldvalues");
            _logRepository.InsertLog("before about me" +request.Values);
            user.SetAboutMe(request.Values.AboutMe);
            _logRepository.InsertLog("after about me");
            user.SetDateModeEnabled(request.Values.DateModeEnabled);
            _logRepository.InsertLog("before profile pictures");
            user.SetProfilePictures(request.Values.ProfilePictures);
            _logRepository.InsertLog("after profile pictures");
            user.SetLocation(request.Values.Location);
            try {
                _logRepository.InsertLog("before update async user");
                await _userRepository.UpdateAsync(user);
                _logRepository.InsertLog("after update async user");
            }
            catch (Exception) {
                _logRepository.InsertLog("Could not update user");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not update user");
            }
            var updateProfileResponse = new UpdateProfileResponse(user.Id, request.Values, oldValues);
            _logRepository.InsertLog("update user success");
            return Request.CreateResponse(HttpStatusCode.Created,
                updateProfileResponse);
        }

        [HttpGet]
        [Route("friends")]
        public async Task<IHttpActionResult> GetFriends(string facebookId) {
            var friendsViewModel = new FriendsViewModel();
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return null;
             //var facebookClient = new FacebookClient($"{Startup.FacebookAuthOptions.AppId}|{Startup.FacebookAuthOptions.AppSecret}")
            var facebookClient = new FacebookClient(String.Format("{0}|{1}", Startup.FacebookAuthOptions.AppId, Startup.FacebookAuthOptions.AppSecret)) {
                AppId = Startup.FacebookAuthOptions.AppId,
                AppSecret = Startup.FacebookAuthOptions.AppSecret
            };
            try {
                var friendIds = new List<string>();
                var friendsData = GetFriendsIds(facebookClient, facebookId,null);
                friendIds.AddRange(friendsData.Friends.Select(x => x.Id));
                while (friendsData.Paging.Cursors.After != null) {
                    friendsData = GetFriendsIds(facebookClient, facebookId, friendsData.Paging.Cursors.After);
                    friendIds.AddRange(friendsData.Friends.Select(x => x.Id));
                }
                var users = _userRepository.Get(friendIds).Where(x => x.DateModeEnabled);
                var friendIdsOfFriends = new List<string>();
                foreach (var user in users) {
                    var friendIdsOfFriend = GetFriendIdsOfFriend(user.FacebookId, facebookClient);
                    friendIdsOfFriends.AddRange(friendIdsOfFriend.Friends.Select(x => x.Id));
                    while (friendIdsOfFriend.Paging.Cursors.After != null) {
                        friendIdsOfFriend = GetFriendIdsOfFriend(user.FacebookId, facebookClient,
                            friendIdsOfFriend.Paging.Cursors.After);
                        friendIdsOfFriends.AddRange(friendIdsOfFriend.Friends.Select(x => x.Id));
                    }
                }
                friendIdsOfFriends = friendIdsOfFriends.Distinct().Where(x => !x.Equals(facebookId)).ToList();
                var friendsOfFriends = _userRepository.Get(friendIdsOfFriends).Where(x => x.DateModeEnabled);
                friendsViewModel.Friends = users.Select(x => x.ToViewModel()).OrderBy(x => x.FirstName);
                friendsViewModel.FriendsOfFriends =
                    friendsOfFriends.Select(x => x.ToViewModel()).OrderBy(x => x.FirstName);
                return Ok(friendsViewModel);
            }
            catch (Exception) {
                return BadRequest("Could not get friends");
            }
        }

        private FacebookFriendData GetFriendIdsOfFriend(string friendFacebookId, FacebookClient client,
            string after = null) {
            dynamic parameters = new ExpandoObject();
            if (after != null) {
                parameters.after = after;
            }
            
           // var response = client.Get($"{friendFacebookId}/friends?fields=installed,id", parameters);
            var response = client.Get(String.Format("{0}/friends?fields=installed,id",friendFacebookId), parameters);
            var facebookFriendData = new FacebookFriendData();
            var friendsData = response["data"] as List<dynamic>;
            if (friendsData != null && friendsData.Count > 0) {
                foreach (var f in friendsData) {
                    var facebookFriend = new FacebookFriend {
                        Id = f["id"],
                        Installed = f["installed"]
                    };
                    facebookFriendData.Friends.Add(facebookFriend);
                }
             //facebookFriendData.Paging.Cursors.After = response["paging"]?["cursors"]?["after"];
                //facebookFriendData.Paging.Cursors.Before = response["paging"]?["cursors"]?["after"];
                 facebookFriendData.Paging.Cursors.After = response["paging"]["cursors"]["after"];
                facebookFriendData.Paging.Cursors.Before = response["paging"]["cursors"]["after"];
            }
            return facebookFriendData;
        }

        private FacebookFriendData GetFriendsIds(FacebookClient client, string facebookId, string after = null) {
            dynamic parameters = new ExpandoObject();
            if (after != null) {
                parameters.after = after;
            }
           // var response = client.Get($"{facebookId}/friends?fields=installed,id", parameters);
            var response = client.Get(String.Format("{0}/friends?fields=installed,id",facebookId), parameters);
            var facebookFriendData = new FacebookFriendData();
            var friendsData = response["data"] as List<dynamic>;
            if (friendsData != null && friendsData.Count > 0) {
                foreach (var friend in friendsData) {
                    var facebookFriend = new FacebookFriend {
                        Id = friend["id"],
                        Installed = friend["installed"]
                    };
                    facebookFriendData.Friends.Add(facebookFriend);
                }
               // facebookFriendData.Paging.Cursors.After = response["paging"]?["cursors"]?["after"];
               // facebookFriendData.Paging.Cursors.Before = response["paging"]?["cursors"]?["before"];
                facebookFriendData.Paging.Cursors.After = response["paging"]["cursors"]["after"];
                facebookFriendData.Paging.Cursors.Before = response["paging"]["cursors"]["before"];
            }
            return facebookFriendData;
        }   
    }
}