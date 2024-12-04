using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace WebBanHang
{
    public class SessionActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy thông tin từ Session
            var userId = context.HttpContext.Session.GetString("D");
            var email = context.HttpContext.Session.GetString("Email");
            var userRole = context.HttpContext.Session.GetString("Role");

            // Kiểm tra xem context.Controller có phải là Controller không
            if (context.Controller is Controller controller)
            {
                // Lưu thông tin vào ViewData
                controller.ViewData["D"] = userId;
                controller.ViewData["Email"] = email;
                controller.ViewData["Role"] = userRole;
            }

            base.OnActionExecuting(context);
        }
    }
}
