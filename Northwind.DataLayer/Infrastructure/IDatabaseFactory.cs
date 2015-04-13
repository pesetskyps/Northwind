using Northwind.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NorthWind.DataLayer.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        NorthwindData Get();
    }
}
