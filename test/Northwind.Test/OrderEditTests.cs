using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Effort.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Northwind.Classes;
using Northwind.DataLayer;
using Northwind.DataLayer.Repositories;
using NorthwindInterfaces.Models;
using NorthWind.DataLayer.Infrastructure;

namespace Northwind.Test
{
    [TestClass]
    public class OrderEditTests
    {
        const int OrderId = 11000;

        readonly Order _newOrderComplex = new Order()
        {
            OrderID = OrderId,
            CustomerID = "RATTC",
            EmployeeID = 2,
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

        [TestInitialize()]
        public void Initialize()
        {
            //Init Effort database
            IDataLoader loader = new CsvDataLoader(@"d:\Repository\Northwind\test\Northwind.Test\FakeDbFiles");
            DbConnection connection = Effort.EntityConnectionFactory.CreatePersistent("name=NorthwindData", loader) as DbConnection;
            var dbcontext = new NorthwindData(connection);
            var dbFactory = new Mock<IDatabaseFactory>();
            dbFactory.Setup(x => x.Get()).Returns(dbcontext);
            //init repos and unit of work
            _realUnitOfWork = new UnitOfWork(dbFactory.Object);
            _realOrderRepository = new OrderRepository(dbFactory.Object);
            _realOrderDetailRepository = new OrderDetailRepository(dbFactory.Object);
        }

        [TestMethod]
        public void Should_Edit_Order_ShipCountry_Attribute()
        {
            //assemble
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
               _realUnitOfWork, _realOrderDetailRepository);
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
               _realUnitOfWork, _realOrderDetailRepository);
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
               _realUnitOfWork, _realOrderDetailRepository);
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
            var orderservice = new OrderService(_realOrderRepository, new DataEntityMapper(),
                          _realUnitOfWork, _realOrderDetailRepository);
            var neworder = _newOrderComplex;
            neworder.OrderID = null; // will be set by EF as it is PK
            orderservice.AddOrder(neworder);
            var count = orderservice.GetOrders().Count();
            Assert.AreEqual(2, count);
        }
    }
}
