using System.Web.Mvc;

namespace Project_64132989.Areas.TrainingOfficer
{
    public class TrainingOfficerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TrainingOfficer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TrainingOfficer_default",
                "TrainingOfficer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}