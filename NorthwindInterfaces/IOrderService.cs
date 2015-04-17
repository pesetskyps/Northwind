using NorthwindInterfaces.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace NorthwindInterfaces
{
    [ServiceContract]
    public interface IOrderService
    {
        [WebGet]
        [OperationContract]
        IEnumerable<Order> GetOrders();
        
        [WebGet]
        [OperationContract]
        void AddOrder(Order order);

        [WebGet]
        [OperationContract]
        void EditOrder(int orderId, Order newOrderDto);
    }
}
