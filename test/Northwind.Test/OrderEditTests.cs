using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using Effort.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Northwind.Classes;
using Northwind.DataLayer;
using Northwind.DataLayer.Repositories;
using Northwind.Infrastructure;
using NorthwindInterfaces.Exceptions;
using NorthwindInterfaces.Models;
using NorthWind.DataLayer.Infrastructure;

namespace Northwind.Test
{
    [TestClass]
    public class OrderEditTests
    {
        const int OrderId = 11000;
        private DateTime now = DateTime.Parse("2015-05-04 00:00:00.000");
        readonly Order _newOrderComplex = new Order()
        {
            OrderID = OrderId,
            CustomerID = "RATTC",
            EmployeeID = 2,
            OrderState = OrderState.New,
            OrderDate = null,
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
            Order_Details = new List<OrderDetail>(){
                new OrderDetail()
                {
                    OrderID = OrderId,ProductID = 4, Discount = Convert.ToSingle(0.25), Quantity = 25, UnitPrice = Convert.ToDecimal(22.00)
                },
                new OrderDetail()
                {
                    OrderID = OrderId,ProductID = 24, Discount = Convert.ToSingle(0.25), Quantity = 30, UnitPrice = Convert.ToDecimal(4.5000)
                },
                new OrderDetail()
                {
                    OrderID = OrderId,ProductID = 77, Discount = Convert.ToSingle(0), Quantity = 30, UnitPrice = Convert.ToDecimal(13.0000)
                },
            }
        };

        private UnitOfWork _realUnitOfWork;
        private OrderRepository _realOrderRepository;
        private OrderDetailRepository _realOrderDetailRepository;
        private Mock<IClock> _clock;

        [TestInitialize()]
        public void Initialize()
        {
            //Init Effort database
            IDataLoader loader = new CsvDataLoader(@"d:\Repository\Northwind\test\Northwind.Test\FakeDbFiles");
            var dbconnection = Effort.EntityConnectionFactory.CreateTransient("name=NorthwindData", loader) as DbConnection;
            var dbcontext = new NorthwindData(dbconnection);
            var dbFactory = new Mock<IDatabaseFactory>();
            dbFactory.Setup(x => x.Get()).Returns(dbcontext);
            //init repos and unit of work
            _realUnitOfWork = new UnitOfWork(dbFactory.Object);
            _realOrderRepository = new OrderRepository(dbFactory.Object);
            _realOrderDetailRepository = new OrderDetailRepository(dbFactory.Object);
            _clock = new Mock<IClock>();
            _clock.Setup(x => x.Now).Returns(now);
        }

        [TestMethod]
        public void Should_Edit_Order_ShipCountry_Attribute()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
               _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var neworder = _newOrderComplex;
            const string newCountry = "Belarus";
            //act
            neworder.ShipCountry = newCountry;
            orderservice.EditOrder(OrderId, neworder);
            //assert
            Assert.AreEqual(newCountry, _realOrderRepository.GetById(OrderId).ShipCountry);
        }
        [TestMethod]
        public void Should_Delete_OrderDetails_Not_In_New_Order()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
               _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var neworder = _newOrderComplex;
            neworder.Order_Details.Remove(neworder.Order_Details.First());
            //act
            var numberOfOrderDetailsBefore = _realOrderDetailRepository.GetAll().Count();
            orderservice.EditOrder(OrderId, neworder);
            var numberOfOrderDetailsAfter = _realOrderDetailRepository.GetAll().Count();
            //assert
            Assert.AreEqual(3, numberOfOrderDetailsBefore);
            Assert.AreEqual(2, numberOfOrderDetailsAfter);
        }

        [TestMethod]
        public void Should_Add_OrderDetails_In_New_Order()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
               _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var neworder = _newOrderComplex;
            neworder.Order_Details.Add(new OrderDetail
            {
                    OrderID = OrderId,
                    ProductID = 1,
                    Discount = Convert.ToSingle(0.25),
                    Quantity = 25,
                    UnitPrice = Convert.ToDecimal(22.00)
                });
            //act
            var numberOfOrderDetailsBefore = _realOrderDetailRepository.GetAll().Count();
            orderservice.EditOrder(OrderId, neworder);
            var numberOfOrderDetailsAfter = _realOrderDetailRepository.GetAll().Count();
            //assert
            Assert.AreEqual(3, numberOfOrderDetailsBefore);
            Assert.AreEqual(4, numberOfOrderDetailsAfter);
        }

        [TestMethod]
        public void Should_Add_Order()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
                          _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var neworder = _newOrderComplex;
            //neworder.OrderID = null; // will be set by EF as it is PK
            //act
            var rsd = _realOrderRepository.GetAll().Count();
            orderservice.AddOrder(neworder);
            
            //assert
            var count = orderservice.GetOrders().Count();
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Should_Delete_Order()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
                          _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var orderToDelete = _newOrderComplex;
            //act
            orderservice.DeleteOrder(orderToDelete);
            var orders = orderservice.GetOrders();
            //assert
            Assert.AreEqual(0, orders.Count());
        }
        
        [TestMethod]
        public void Should_Move_State_Modified_OrderDate()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
                          _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var order = _newOrderComplex;
            //act
            orderservice.MoveOrderToState(OrderState.InWork, order);
            //assert
            var modifiedOrder = _realOrderRepository.GetById(order.OrderID);

            Assert.AreEqual(now, modifiedOrder.OrderDate);
        }

        [TestMethod]
        public void Should_Move_State_Modified_ShippedDate()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
                          _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var order = _newOrderComplex;
            order.OrderDate = now.AddDays(-1);
            order.OrderState = OrderState.InWork;
            //act
            orderservice.MoveOrderToState(OrderState.Shipped, order);
            //assert
            var modifiedOrder = _realOrderRepository.GetById(order.OrderID);

            Assert.AreEqual(now, modifiedOrder.ShippedDate);
        }

        [ExpectedException(typeof(FaultException<InvalidOrderChangeException>))]
        [TestMethod]
        public void Should_Throw_Modifying_State_From_New_To_Shipped()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
                          _realUnitOfWork, _realOrderDetailRepository, _clock.Object);
            var order = _newOrderComplex;

            //act
            orderservice.MoveOrderToState(OrderState.Shipped, order);
            //assert
            var modifiedOrder = _realOrderRepository.GetById(order.OrderID);

            Assert.AreEqual(now, modifiedOrder.ShippedDate);
        }
    }
}
