using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Optimization;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using NHibernate;

namespace xpw.Web
{
    public class AppConfig
    {
        private static IWindsorContainer _container;

        public static void Initialize()
        {
            CreateContainer();
            RegisterControllerFactory();
            InitializeNHibernate();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, _container);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //RegisterRepositories();
            //RegisterQueries();

            //MetadataBuilderConfig.Start(_container);
        }

        public static void Dispose()
        {
            _container.Dispose();
        }

        private static void CreateContainer()
        {
            _container = new WindsorContainer();

            // register the container with itself to be able to resolve 
            // the dependency in the ctor of WindsorControllerFactory
            _container.Register(Component.For<IWindsorContainer>().
                Instance(_container).LifeStyle.Singleton);
        }

        private static void RegisterControllerFactory()
        {
            _container.Register(
                Component.For<IControllerFactory>()
                    .ImplementedBy<WindsorControllerFactory>()
                    .LifeStyle.Singleton);

            ControllerBuilder.Current.SetControllerFactory(_container.Resolve<IControllerFactory>());
        }

        private static void InitializeNHibernate()
        {
            var inititializer = new NHibernateConfig();
            inititializer.InitializeForWeb();
            inititializer.UpdateSchema();
            //inititializer.CreateSchema();

            _container.Register(
                Component.For<ISessionFactory>()
                    .Instance(inititializer.SessionFactory)
                    .LifeStyle.Singleton);

            _container.Register(
                Component.For<SessionPerActionFilter>()
                    .ImplementedBy<SessionPerActionFilter>()
                    .LifeStyle.Transient);
        }

        //private static void RegisterRepositories()
        //{
        //    _container.Register(
        //        Component.For(typeof(IRepository<>))
        //            .ImplementedBy(typeof(Repository<>))
        //            .LifeStyle.Transient);
        //}

        //private static void RegisterQueries()
        //{
        //    _container.Register(AllTypes
        //        .FromAssembly(Assembly.GetAssembly(typeof(Query<>)))
        //        .BasedOn(typeof(IQuery<>))
        //        .WithService.AllInterfaces());
        //}

        //private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //{
        //    //filters.Add(new RequestTimingFilter(), 1);
        //    filters.Add(new HandleErrorAttribute(), 2);
        //    filters.Add(_container.Resolve<SessionPerActionFilter>(), 3);
        //}
    }
}