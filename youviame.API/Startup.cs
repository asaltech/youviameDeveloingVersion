using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.OAuth;
using Owin;
using youviame.Config;
using Configuration = youviame.Data.Migrations.Configuration;

[assembly: OwinStartup(typeof(youviame.API.Startup))]
namespace youviame.API {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            ConfigureOAuth(app);
            ConfigureContainer(httpConfiguration);
            ConfigureDatabase();
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(httpConfiguration);
        }
    }

    public partial class Startup {

        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }
        public void ConfigureOAuth(IAppBuilder app) {
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            FacebookAuthOptions = new FacebookAuthenticationOptions {
                AppId = "1685342265085833",
                AppSecret = "d817649f1b09be20baca076fd5eb32a5",
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(FacebookAuthOptions);
            var oAuthServerOptions = new OAuthAuthorizationServerOptions {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan =  TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }

    public partial class Startup {

        public void ConfigureContainer(HttpConfiguration config) {
            var container = ContainerInstaller.Container;
            config.DependencyResolver = new UnityResolver(container);

           
        }
    }

    public partial class Startup {
        public void ConfigureDatabase() {
            if (bool.Parse(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"])) {
                var configuration = new Configuration();
                var dbMigrator = new DbMigrator(configuration);
                dbMigrator.Update();
            }
        }
    }
}