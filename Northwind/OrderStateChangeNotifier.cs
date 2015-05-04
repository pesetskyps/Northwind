using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using NorthwindInterfaces;
using NorthwindInterfaces.Exceptions;

namespace Northwind
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class OrderStateChangeNotifier : IOrderStateChangeNotifierService
    {
        static Dictionary<int, List<IOrderStateChangeCallback>> callbacksDict = new Dictionary<int, List<IOrderStateChangeCallback>>();

        public void SubcribeToOrderStateChange(int orderId)
        {
            Console.WriteLine("> Session opened at {0}", DateTime.Now);
            IOrderStateChangeCallback callback =
                OperationContext.Current.GetCallbackChannel<IOrderStateChangeCallback>();
            if (!callbacksDict.ContainsKey(orderId))
            {
                callbacksDict.Add(orderId, new List<IOrderStateChangeCallback> { callback });
            }
            else
            {
                List<IOrderStateChangeCallback> callbacks;
                if (callbacksDict.TryGetValue(orderId, out callbacks))
                {
                    if (!callbacks.Contains(callback))
                    {
                        callbacks.Add(callback);
                        callbacksDict[orderId] = callbacks;
                    }
                }
            }
        }

        public void SendOrderStateChange(int orderId)
        {
            List<IOrderStateChangeCallback> callbacks;
            if (callbacksDict.TryGetValue(orderId, out callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback.OnOrderStateChange(string.Format("Order state changed for the order {0}", orderId));
                }
            }
        }
    }
}
