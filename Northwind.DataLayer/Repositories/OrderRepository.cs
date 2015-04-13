﻿using NorthWind.DataLayer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.DataLayer.Repositories
{
    public class OrderRepository : RepositoryBase<OrderEntity>
    {
        public OrderRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
}
