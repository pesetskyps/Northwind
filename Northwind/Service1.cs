using AutoMapper;
using NorthwindInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Entity;
using Northwind.Classes;
using Northwind.DataLayer;
using NorthWind.DataLayer.Infrastructure;
using NorthwindInterfaces;
namespace Northwind
{
    public class Service1 : IService1
    {
        IRepository<OrderEntity> _repository;
        public Service1()
        {

        }
        public Service1(IRepository<OrderEntity> repository)
        {
            _repository = repository;
        }
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public IEnumerable<Order> GetOrders()
        {
            //var context = new NorthwindData();
            //var ordersEf = context.OrderEntities.Include(x => x.Customer).OrderBy(x => x.OrderID).ToList();
            var rr = _repository.GetAll().ToList();
            Mapper.CreateMap<OrderEntity, Order>();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<OrderDetailEntity, OrderDetail>();
            Mapper.CreateMap<CustomerEntity, Customer>();
            IEnumerable<Order> orders = Mapper.Map<List<OrderEntity>, List<Order>>(rr);
            //OrderEnricher<Order> enricher = new OrderEnricher<Order>();
            //orders = enricher.Enrich(orders);
            return orders.Take(10);
        }
    }
}
