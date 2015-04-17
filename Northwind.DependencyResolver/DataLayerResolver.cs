using Autofac;
using Northwind.DataLayer;
using Northwind.DataLayer.Repositories;
using NorthWind.DataLayer.Infrastructure;

namespace Northwind.DependencyResolver
{
    public class DataLayerResolver : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<OrderRepository>().As<IRepository<OrderEntity>>();
        }
    }
}
