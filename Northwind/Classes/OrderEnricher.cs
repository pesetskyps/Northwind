using NorthwindInterfaces.Models;
using System.Collections.Generic;
using System.Linq;


namespace Northwind.Classes
{
    public interface IOrderEnricher<T>
    {
        List<T> Enrich(IEnumerable<T> entity);
        T Enrich(T entity);
    }
    public class OrderEnricher<T> : IOrderEnricher<T> where T: Order
    {
        public List<T> Enrich(IEnumerable<T> orders)
        {
            List<T> enrichedOrders = new List<T>();
            foreach (var order in orders.ToList())
            {
                if (order.OrderDate == null)
                    order.OrderState = OrderState.New;
                else
                    order.OrderState = order.ShippedDate == null ? OrderState.InWork : OrderState.Shipped;
                enrichedOrders.Add(order);
            }
            return enrichedOrders;
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
