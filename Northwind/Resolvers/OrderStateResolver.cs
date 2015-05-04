using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataLayer;
using NorthwindInterfaces.Models;

namespace Northwind.Resolvers
{
    class OrderStateResolver : ValueResolver<OrderEntity, OrderState>
    {
        protected override OrderState ResolveCore(OrderEntity order)
        {
            if (order.OrderDate == null)
                return OrderState.New;
            return order.ShippedDate == null ? OrderState.InWork : OrderState.Shipped;
        }
    }
}
