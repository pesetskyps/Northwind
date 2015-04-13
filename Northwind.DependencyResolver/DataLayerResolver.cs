using Autofac;
using Northwind.DataLayer;
using Northwind.DataLayer.Repositories;
using NorthWind.DataLayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.DependencyResolver
{
    public class DataLayerResolver : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<OrderRepository>().As<IRepository<OrderEntity>>();
        }
    }
}
