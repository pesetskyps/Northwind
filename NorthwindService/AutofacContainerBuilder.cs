using Autofac;
using Northwind;
using Northwind.DependencyResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthwindService
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // register types
            builder.RegisterModule<DataLayerResolver>();
            builder.RegisterType<Service1>().AsSelf();
            builder.RegisterType<OrderService>().AsSelf();
            // build container
            return builder.Build();
        }
    }
}