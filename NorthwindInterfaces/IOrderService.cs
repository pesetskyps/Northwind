using NorthwindInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindInterfaces
{
    [ServiceContract]
    public interface IOrderService
    {
        [WebGet]
        [OperationContract]
        IEnumerable<Order> GetOrders();
    }
}
