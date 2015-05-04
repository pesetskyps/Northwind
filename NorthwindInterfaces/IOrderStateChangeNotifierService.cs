using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindInterfaces
{
    [ServiceContract(CallbackContract = typeof(IOrderStateChangeCallback))]
    public interface IOrderStateChangeNotifierService
    {
        [OperationContract]
        void SubcribeToOrderStateChange(int orderId);

        [OperationContract]
        void SendOrderStateChange(int orderId);
    }
}
