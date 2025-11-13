using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    [Route("/API/Action")] // у API контролері один маршрут, назва методів ролі не грає
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet] // роль грає Http-метод запиту, саме за ним відбувається розгалуженння
        public string GetAction()
        {
            return "ApiController::GetAction";
        }

        [HttpPost] // якщо не зазначати атрибут, то метод буде працювати для всіх запитів, а поява другого методу призведе до помилки неоднозначності
        public object PostAction()
        {
            return new
            {
                controller = "ApiController",
                action = "PostAction",
            };
        }

        [HttpGet("sub")] // створення внутрішньої адреси /API/Action/sub
        public string GetSubAction()
        {
            return "ApiController::GetSubAction";
        }

        [HttpGet("users")]
        public object GetUsersAction()
        {
            return new
            {
                user1 = "Mathew",
                user2 = "Jack",
                user3 = "Tom"
            };
        }

        [HttpPatch("sub")] // створення внутрішньої адреси /API/Action/sub для метода PATCH
        public string PatchSubAction()
        {
            return "ApiController::PatchSubAction";
        }

        [HttpDelete("user")] // створення внутрішньої адреси /API/Action/user для метода DELETE
        public string DeleteUserAction()
        {
            return "ApiController::DeleteUserAction";
        }
    }
}
