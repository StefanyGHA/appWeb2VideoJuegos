using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace appWeb2.Filtros
{
	public class AdminAuthorize : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var rolId = context.HttpContext.Session.GetInt32("Idrol");

			if (rolId != 1)
			{
				context.Result = new RedirectToActionResult("Login", "Account", null);
			}
		}
	}
}