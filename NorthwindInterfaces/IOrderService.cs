using NorthwindInterfaces.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using NorthwindInterfaces.Exceptions;

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
        [FaultContract(typeof(InvalidOrderChangeException))]
        void EditOrder(int orderId, Order newOrderDto);

        [OperationContract]
        [FaultContract(typeof(InvalidOrderChangeException))]
        void DeleteOrder(Order newOrderDto);
    }
}
