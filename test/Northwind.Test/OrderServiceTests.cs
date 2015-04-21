using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NorthWind.DataLayer.Infrastructure;
using Northwind.DataLayer;
using NorthwindInterfaces.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extras.Moq;
using Northwind.Classes;
using Northwind.DataLayer.Repositories;
using Northwind.Exceptions;
using System.ServiceModel;

namespace Northwind.Test
{
    [TestClass]
    public class OrderServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IRepository<OrderEntity>> _mockRepository;
        private Mock<IOrderDetailRepository> _mockOrderDetailRepository;

        const int _orderId = 11000;
        readonly OrderEntity _complexOrderEntityShippedState = new OrderEntity()
        {
            OrderID = _orderId,
            CustomerID = "RATTC",
            EmployeeID = 2,
            OrderDate = DateTime.Parse("1998-04-06 00:00:00.000"),
            RequiredDate = DateTime.Parse("1998-05-04 00:00:00.000"),
            ShippedDate = DateTime.Parse("1998-04-14 00:00:00.000"),
            ShipVia = 3,
            Freight = Decimal.Parse("55.12"),
            ShipName = "Rattlesnake Canyon Grocery",
            ShipAddress = "2817 Milton Dr.",
            ShipCity = "Albuquerque",
            ShipRegion = "NM",
            ShipPostalCode = "87110",
            ShipCountry = "USA",
            Order_Details = new List<OrderDetailEntity>(){new OrderDetailEntity()
                {
                    OrderID = _orderId,ProductID = 4, Discount = Convert.ToSingle(0.25), Quantity = 25, UnitPrice = Convert.ToDecimal(22.00),
                    Product = new ProductEntity()
                    {
                        ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", SupplierID = 2, CategoryID = 2, QuantityPerUnit = "48 - 6 oz jars",
                        UnitPrice = Convert.ToDecimal(22.00), UnitsInStock = 53, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false
                    }
                }}
        };

        [TestInitialize()]
        public void Initialize()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IRepository<OrderEntity>>();
            _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();


        }

        [TestMethod]
        public void GetOrders_Should_Return_Order()
        {
            //assemble
            IEnumerable<OrderEntity> mockorder = new List<OrderEntity> { new OrderEntity() { OrderID = 56 } };
            _mockRepository.Setup(x => x.GetAll()).Returns(mockorder);
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);

            //Act
            var orders = orderservice.GetOrders();

            //assert
            Assert.AreEqual(56, orders.First().OrderID);
        }

        [TestMethod]
        public void Should_Add_Order()
        {
            //assemble
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            var order = new Order() { OrderID = 56 };
            //act
            orderservice.AddOrder(order);

            //assert
            _mockRepository.Verify(lw => lw.Add(It.IsAny<OrderEntity>()), Times.Once());
        }

        [TestMethod]
        public void Should_Edit_Order_ShipAddress_Attribute()
        {
            //assemble          
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            _mockRepository.Setup(x => x.GetById(_orderId)).Returns(_complexOrderEntityShippedState);
            var newOrder = new Order() { ShipAddress = "Vitebsk" };

            //act
            orderservice.EditOrder(_orderId, newOrder);
            //assert
            _unitOfWork.Verify(lw => lw.Commit(), Times.Once());
        }

        [ExpectedException(typeof(FaultException<InvalidOrderChangeException>))]
        [TestMethod]
        public void Should_Throw_Exception_Editing_OrderDate()
        {
            //assemble
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            Order newOrder = new Order() { OrderDate = DateTime.Parse("1998-04-06 00:00:00.000") };

            //act
            orderservice.EditOrder(_orderId, newOrder);
        }

        [ExpectedException(typeof(FaultException<InvalidOrderChangeException>))]
        [TestMethod]
        public void Should_Throw_Exception_Editing_ShippedDate()
        {
            //assemble
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            Order newOrder = new Order() { ShippedDate = DateTime.Parse("1998-04-06 00:00:00.000") };

            //act
            orderservice.EditOrder(_orderId, newOrder);
        }

        [ExpectedException(typeof(FaultException<InvalidOrderChangeException>))]
        [TestMethod]
        public void Should_Throw_Exception_Editing_OrderState()
        {
            //assemble
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            Order newOrder = new Order() { OrderState = OrderState.Shipped};

            //act
            orderservice.EditOrder(_orderId, newOrder);
        }

        [ExpectedException(typeof(FaultException<InvalidOrderChangeException>))]
        [TestMethod]
        public void Should_Throw_Exception_NewOrder_IsNull()
        {
            //assemble
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            //act
            orderservice.EditOrder(_orderId, null);
        }

        [ExpectedException(typeof(FaultException<InvalidOrderChangeException>))]
        [TestMethod]
        public void Should_Throw_Exception_onEdit_OrderState_IsNot_In_New_State()
        {
            //assemble
            var orderservice = new OrderService(_mockRepository.Object, new DataEntityMapper(), _unitOfWork.Object, _mockOrderDetailRepository.Object);
            _mockRepository.Setup(x => x.GetById(_orderId)).Returns(_complexOrderEntityShippedState);
            var newOrder = new Order() { ShipAddress = "Vitebsk" };
            //act
            orderservice.EditOrder(_orderId, newOrder);
        }
    }
}
