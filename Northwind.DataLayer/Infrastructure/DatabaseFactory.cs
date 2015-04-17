using Northwind.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NorthWind.DataLayer.Infrastructure
{
public class DatabaseFactory : Disposable, IDatabaseFactory
{
    private NorthwindData dataContext;
    public virtual NorthwindData Get()
    {
        return dataContext ?? (dataContext = new NorthwindData());
    }
    protected override void DisposeCore()
    {
        if (dataContext != null)
            dataContext.Dispose();
    }
}
}
