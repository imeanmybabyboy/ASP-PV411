using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP_PV411.Models.Home
{
    public class HomeDemoViewModel
    {
        public string PageTitle { get; set; } = string.Empty;
        public string FormTitle { get; set; } = string.Empty;
        public HomeDemoFormModel? FormModel { get; set; }
        public Dictionary<string, string>? ModelErrors { get; set; }
    }
}