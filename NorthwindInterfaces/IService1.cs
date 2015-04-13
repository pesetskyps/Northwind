using NorthwindInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NorthwindInterfaces
{
    [ServiceContract]
    public interface IService1
    {
        [WebGet]
        [OperationContract]
        string GetData(int value);

        // TODO: Add your service operations here
        [WebGet]
        [OperationContract]
        IEnumerable<Order> GetOrders();
    }
}
