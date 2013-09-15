using System.Web;
using System.Web.Mvc;
using Castle.Windsor;

namespace xpw.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IWindsorContainer container)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(container.Resolve<SessionPerActionFilter>(), 3);
        }
    }
}