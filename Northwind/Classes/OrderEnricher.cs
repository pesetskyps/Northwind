using NorthwindInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Classes
{
    public interface IOrderEnricher<T>
    {
        IEnumerable<T> Enrich(IEnumerable<T> entity);
    }
    public class OrderEnricher<T> : IOrderEnricher<T> where T: Order
    {
        public IEnumerable<T> Enrich(IEnumerable<T> orders)
        {
            foreach (var order in orders)
            {
                if (order.OrderDate == null)
                {
                    order.OrderState = OrderState.New;
                }
                else
                {
                    if (order.ShippedDate == null)
                    {
                        order.OrderState = OrderState.InWork;
                    }
                    else
                    {
                        order.OrderState = OrderState.Shipped;
                    }
                }
            }
            return orders;
        }
    }
}
