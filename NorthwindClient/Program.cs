using NorthwindInterfaces;
using NorthwindInterfaces.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using NorthwindInterfaces.Exceptions;


namespace NorthwindClient
{
    class Program
    {
        static void Main()
        {
            //ChannelFactory<IService1> servicefactory = new ChannelFactory<IService1>("Service1");
            var binding = new WSDualHttpBinding();
            var servicefactory = new ChannelFactory<IOrderService>(binding,"OrderService");
            var channel = servicefactory.CreateChannel();
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
            var newOrderComplex = new Order
            {
                OrderID = 11000,
                CustomerID = "RATTC",
                EmployeeID = 2,
                OrderDate = null,
                OrderState = OrderState.New,
                RequiredDate = DateTime.Parse("1998-05-04 00:00:00.000"),
                ShippedDate = null,
                ShipVia = 3,
                Freight = Decimal.Parse("55.12"),
                ShipName = "Rattlesnake Canyon Grocery",
                ShipAddress = "2817 Milton Dr.",
                ShipCity = "Albuquerque",
                ShipRegion = "NM",
                ShipPostalCode = "87110",
                ShipCountry = "USA",
                Order_Details = new List<OrderDetail>()
                {new OrderDetail
                    {
                        OrderID = 11000,ProductID = 4, Discount = Convert.ToSingle(0.25), Quantity = 25, UnitPrice = Convert.ToDecimal(22.00)
                    },
                    new OrderDetail
                    {
                        OrderID = 11000,ProductID = 25, Discount = Convert.ToSingle(0.25), Quantity = 25, UnitPrice = Convert.ToDecimal(22.00)
                    }
                }
            };
            try
            {
                channel.MoveOrderToState(OrderState.InWork, newOrderComplex);
            }
            catch (FaultException<InvalidOrderChangeException> ex)
            {

                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            Console.ReadLine();
        }
    }
}
