using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace appWeb2.Filtros
{
	public class SessionAuthorize: ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var usuario = context.HttpContext.Session.GetString("usuario");
			if (usuario == null) 
			{
				context.Result = new RedirectToActionResult("Login", "Account", null);
			}
		}

		public class AdminAuthorize : ActionFilterAttribute
		{
			public override void OnActionExecuting(ActionExecutingContext context)
			{
				var rolId = context.HttpContext.Session.GetInt32("rolId");

				if (rolId != 1)
				{
					context.Result = new RedirectToActionResult("Login", "Account", null);
				}
			}
		}
	}
}
