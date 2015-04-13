using NorthwindInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //ChannelFactory<IService1> servicefactory = new ChannelFactory<IService1>("Service1");
            ChannelFactory<IOrderService> servicefactory = new ChannelFactory<IOrderService>("OrderService");
            var channel =servicefactory.CreateChannel();
            Console.ReadLine();
            Console.WriteLine(channel.GetOrders());
            Console.ReadLine();
        }
    }
}
