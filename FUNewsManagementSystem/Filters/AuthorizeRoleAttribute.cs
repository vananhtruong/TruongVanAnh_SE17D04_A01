using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FUNewsManagementSystem.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var userRole = httpContext.Session.GetString("UserRole");
            var userEmail = httpContext.Session.GetString("UserEmail");

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userRole))
            {
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
                return;
            }

            // Kiểm tra xem vai trò của người dùng có nằm trong danh sách allowedRoles không
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Authentication", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}