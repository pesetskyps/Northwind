using NorthwindInterfaces;
using NorthwindInterfaces.Models;
using System;
using System.ServiceModel;


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
            //var orders = channel.GetOrders();
            //if (orders != null)
            //    foreach (var item in orders)
            //    {
            //        foreach (var orderDetail in item.Order_Details)
            //        {
            //            Console.WriteLine("Order {0} with state {1} with Product {2} with CategoryID {3}", item.OrderID, item.OrderState, orderDetail.Product.ProductName, orderDetail.Product.CategoryID);
            //        }
                
            //    }
            //Console.WriteLine("Add the order y/n?");
            //string line = Console.ReadLine();
            //if (line == "y")
            //{
            //    channel.AddOrder(new Order(){ShipCity="Minsk"});
            //}
            channel.EditOrder(11000,new Order(){ShipAddress = "Brest", EmployeeID = 1});

            Console.ReadLine();
        }
    }
}
