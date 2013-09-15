using System;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Routing;
using Castle.Windsor;
using Castle.MicroKernel.Registration;

namespace xpw.Web
{
    // http://blog.andreloker.de/post/2009/03/28/ASPNET-MVC-with-Windsor-programmatic-controller-registration.aspx
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        readonly IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            _container = container;

            RegisterControllers();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return base.GetControllerInstance(requestContext, controllerType);

            return _container.Resolve(controllerType) as IController;
        }

        public override void ReleaseController(IController controller)
        {
            _container.Release(controller);
        }

        private void RegisterControllers()
        {
            _container.Register(
                Classes.FromAssembly(Assembly.GetExecutingAssembly()).
                BasedOn<IController>().
                LifestyleTransient());
        }
    }
}