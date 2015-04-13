using AutoMapper;
using Northwind.DataLayer;
using NorthWind.DataLayer.Infrastructure;
using NorthwindInterfaces;
using NorthwindInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Northwind
{
    public class OrderService : IOrderService
    {
        IRepository<OrderEntity> _repository;
        public OrderService()
        {

        }
        public OrderService(IRepository<OrderEntity> repository)
        {
            _repository = repository;
        }
        public IEnumerable<Order> GetOrders()
        {
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
