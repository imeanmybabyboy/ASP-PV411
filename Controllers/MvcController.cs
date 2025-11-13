using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class MvcController : Controller
    {
        public IActionResult Action()
        {
            return Content("MvcController::Action");
        }

        [HttpPost] // на відміну від API, даний атрибут лише обмежує методи доступу до дії (action)
        public IActionResult PostAction()
        {
            return Content("MvcController::PostAction");
        }

        [HttpGet] // внутрішні адреси MVC - через параметр [FromRoute] string id
        public IActionResult GetAction([FromRoute] string id)
        {
            return Content("MvcController::GetAction / " + id);
        }
    }
}
