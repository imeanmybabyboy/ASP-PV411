using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    [Route("/api/info")]
    [ApiController]
    public class ApiInfoController : ControllerBase
    {
        [HttpGet]
        public string GetApiInfo()
        {
            return "ApiInfoController::GetMvc";
        }

        [HttpPost]
        public string PostApiInfo()
        {
            return "ApiInfoController:PostApi";
        }
    }
}
