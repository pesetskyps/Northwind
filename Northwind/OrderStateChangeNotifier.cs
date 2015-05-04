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
    class OrderStateChangeNotifier : IOrderStateChangeNotifierService
    {
        static List<IOrderStateChangeCallback> mCallbacks = new List<IOrderStateChangeCallback>();

        public void SubcribeToOrderStateChange(string message)
        {
            Console.WriteLine("> Session opened at {0}", DateTime.Now);
            IOrderStateChangeCallback callback = OperationContext.Current.GetCallbackChannel<IOrderStateChangeCallback>();
            if (!mCallbacks.Contains(callback))
            {
                mCallbacks.Add(callback);
            }

            mCallbacks.ForEach(t => t.OnOrderStateChange(message));
        }
    }
}
