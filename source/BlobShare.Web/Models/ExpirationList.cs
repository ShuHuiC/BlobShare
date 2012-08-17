namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class ExpirationList : List<SelectListItem>
    {
        public ExpirationList()
        {
            this.Add(new SelectListItem() { Value = "1", Text = "1 Day" });
            this.Add(new SelectListItem() { Value = "2", Text = "2 Days" });
            this.Add(new SelectListItem() { Value = "3", Text = "3 Days" });
            this.Add(new SelectListItem() { Value = "7", Text = "7 Days" });
            this.Add(new SelectListItem() { Value = "14", Text = "14 Days" });
            this.Add(new SelectListItem() { Value = "30", Text = "1 Month" });
            this.Add(new SelectListItem() { Value = "60", Text = "2 Months" });
            this.Add(new SelectListItem() { Value = "90", Text = "3 Months" });
            this.Add(new SelectListItem() { Value = "120", Text = "4 Months" });
            this.Add(new SelectListItem() { Value = "150", Text = "5 Months" });
            this.Add(new SelectListItem() { Value = "180", Text = "6 Months" });
            this.Add(new SelectListItem() { Value = "360", Text = "1 Year" });
        }
    }
}