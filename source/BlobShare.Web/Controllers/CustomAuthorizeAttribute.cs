namespace Microsoft.Samples.DPE.BlobShare.Web
{
    using System.Web.Mvc;

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/403.htm");
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}