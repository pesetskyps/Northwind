using Autofac;
using Northwind;
using Northwind.DependencyResolver;

namespace NorthwindService
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // register types
            builder.RegisterModule<DataLayerResolver>();
            builder.RegisterModule<NorthwindLibraryResolver>();
            builder.RegisterType<OrderService>().AsSelf();
            // build container
            return builder.Build();
        }
    }
}