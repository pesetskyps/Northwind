using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindInterfaces.Models;
using System.Collections.Generic;
using System.Linq;
using Northwind.Classes;

namespace Northwind.Test
{
    [TestClass]
    public class EnricherTests
    {
        [TestMethod]
        public void Enricher_Should_Return_New_State_For_Null_OrderDate()
        {
            //assemble
            IEnumerable<Order> mockorder = new List<Order> { new Order() { OrderDate = null } };
            var enricher = new OrderEnricher<Order>();
            
            //act
            var enrichedOrder =enricher.Enrich(mockorder);
            //assert
            Assert.AreEqual(OrderState.New, enrichedOrder.First().OrderState);
        }
    }
}
