using Autofac;
using Northwind.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.DependencyResolver
{
    public class NorthwindLibraryResolver : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataEntityMapper>().As<IDataEntityMapper>();
        }
    }
}
