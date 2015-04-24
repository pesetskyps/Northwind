using NorthwindInterfaces.Models;
using System.Collections.Generic;
using System.Linq;


namespace Northwind.Classes
{
    public class OrderEnricher<T> : IOrderEnricher<T> where T: Order
    {
        public List<T> Enrich(IEnumerable<T> orders)
        {
            return orders.ToList().Select(Enrich).ToList();
        }

        public T Enrich(T order)
        {
                if (order.OrderDate == null)
                    order.OrderState = OrderState.New;
                else
                    order.OrderState = order.ShippedDate == null ? OrderState.InWork : OrderState.Shipped;
            return order;
        }
    }
}
