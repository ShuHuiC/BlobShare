namespace Microsoft.Samples.DPE.BlobShare.Web
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Web;
    using Microsoft.IdentityModel.Web.Configuration;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Web.Controllers;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "ViewBlob",
                "MyBlobs/ViewBlob/{BlobId}",
                new { controller = "MyBlobs", action = "ViewBlob", BlobId = UrlParameter.Optional });

            routes.MapRoute(
                "DownloadBlob",
                "MyBlobs/DownloadBlob/{BlobId}",
                new { controller = "MyBlobs", action = "DownloadBlob", BlobId = UrlParameter.Optional });

            routes.MapRoute(
                "ViewBlobSet",
                "MyBlobs/ViewBlobSet/{blobSetId}",
                new { controller = "MyBlobs", action = "ViewBlobSet", blobSetId = UrlParameter.Optional });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            FederatedAuthentication.ServiceConfigurationCreated += this.OnServiceConfigurationCreated;

            Microsoft.WindowsAzure.CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {                
                configSetter(ConfigurationManager.AppSettings[configName]);
            });

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            if (exception is HttpException && ((HttpException)exception).WebEventCode == 3004)
            {
                Server.ClearError();
                Response.Redirect("Upload?FileTooLarge=", true);
                return;
            }

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            routeData.Values.Add("action", "Index");

            routeData.Values.Add("error", exception);

            Server.ClearError();

            IController errorController = new ErrorController();
            try
            {
                errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
            finally
            {
                ((ErrorController)errorController).Dispose();
            }

            if (Response.IsClientConnected)
            {
                Response.End();
            }
        }

        protected void Application_BeginRequest()
        {
            // Uncomment this when using HTTPS
            ////if (!this.Request.IsSecureConnection)
            ////{
            ////    this.Response.Redirect(string.Format(CultureInfo.InvariantCulture, "https://{0}{1}", this.Context.Request.Url.Host, this.Context.Request.Url.PathAndQuery), true);
            ////}

            if (ConfigurationManager.AppSettings["HasAdmin"].Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
            {
                if (new UserService().GetUsers().Count() == 0)
                {
                    if (!Request.Path.StartsWith("/Account/") 
                        && !Request.Path.StartsWith("/Content/")
                        && !Request.Path.StartsWith("/Scripts/"))
                    {
                        Response.Redirect("~/Account/RegisterAdmin");
                    }
                }
                else
                {
                    ConfigurationManager.AppSettings["HasAdmin"] = bool.TrueString;
                }
            }
        }

        private void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
        {
            // Use the <serviceCertificate> to protect the cookies that are sent to the client.
            List<CookieTransform> sessionTransforms =
              new List<CookieTransform>(
                  new CookieTransform[] 
                  { 
                      new DeflateCookieTransform(), 
                      new RsaEncryptionCookieTransform(this.GetCertificate()),
                      new RsaSignatureCookieTransform(this.GetCertificate())  
                  });

            var sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());
            e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
        }

        private X509Certificate2 GetCertificate()
        {
            var certificateLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "blobshare.cloudapp.net.pfx");
            return new X509Certificate2(certificateLocation, "abc!123");
        }
    }
}