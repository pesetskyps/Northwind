using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthWind.DataLayer.Infrastructure;

namespace Northwind.DataLayer.Repositories
{
    public class OrderDetailRepository : RepositoryBase<OrderDetailEntity>
    {
        public OrderDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
             
        }
    }
}
