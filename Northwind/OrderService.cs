using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using AutoMapper;
using Northwind.Classes;
using Northwind.DataLayer;
using Northwind.Exceptions;
using Northwind.Helpers;
using NorthwindInterfaces;
using NorthwindInterfaces.Models;
using NorthWind.DataLayer.Infrastructure;

namespace Northwind
{
    public class OrderService : IOrderService
    {
        readonly IRepository<OrderEntity> _repository;
        readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OrderDetail> _repositoryOrderDetail;
        public OrderService()
        {

        }
        public OrderService(IRepository<OrderEntity> repository, IDataEntityMapper mapper, IUnitOfWork unitOfWork//, IRepository<OrderDetail>
            )
        {
            _repository = repository;
            //_repositoryOrderDetail = _repositoryOrderDetail;
            _unitOfWork = unitOfWork;
            mapper.CreateMap();
        }
        public IEnumerable<Order> GetOrders()
        {
            var databaseOrders = _repository.GetAll().OrderByDescending(x => x.OrderID).ToList();

            IEnumerable<Order> orders = Mapper.Map<List<OrderEntity>, List<Order>>(databaseOrders);
            OrderEnricher<Order> enricher = new OrderEnricher<Order>();
            orders = enricher.Enrich(orders);
            return orders.Take(10);
        }

        public void AddOrder(Order order)
        {
            var databaseOrder = Mapper.Map<Order, OrderEntity>(order);
            _repository.Add(databaseOrder);
            _unitOfWork.Commit();
        }

        public void EditOrder(int orderId, Order newOrderDto)
        {
            if (newOrderDto == null)
            {
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "There are no changes received for the order." });
            }

            if (orderId == null)
            {
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "OrderID is empty. Please specify the order to change" });
            }
            var orderToUpdateEntity = _repository.GetById(orderId);

            if (orderToUpdateEntity != null)
            {
                var enricher = new OrderEnricher<Order>();
                var order = Mapper.Map<OrderEntity, Order>(orderToUpdateEntity);
                var enrichedOrderToUpdate = enricher.Enrich(order);
                var enrichedNewOrderDto = enricher.Enrich(newOrderDto);
                if (enrichedNewOrderDto.OrderDate != enrichedOrderToUpdate.OrderDate ||
                    enrichedNewOrderDto.ShippedDate != enrichedOrderToUpdate.ShippedDate ||
                    enrichedNewOrderDto.OrderState != enrichedOrderToUpdate.OrderState)
                {
                    throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "OrderID, OrderDate, ShippedDate, OrderState fields couldn't be changed directly" });
                }
                if (enrichedOrderToUpdate.OrderState == OrderState.Shipped)
                {
                    Mapper.Map(newOrderDto, orderToUpdateEntity);

                    //delete deleted orderdetail
                    //remove deleted details
                    //var tt = _repositoryOrderDetail.GetMany(orderOrm => orderOrm.OrderID != newOrderDto.OrderID))
                    var ordersToDelte = orderToUpdateEntity.Order_Details
                    .Where(orderOrm => !enrichedNewOrderDto.Order_Details
                        .All(detailDto => detailDto.OrderID == orderOrm.OrderID));
                    ordersToDelte.Each(deleted => orderToUpdateEntity.Order_Details.Remove(deleted));

                    //update orderdetail
                    enrichedNewOrderDto.Order_Details.Each(detailDto =>
                    {
                        var detail = orderToUpdateEntity.Order_Details
                                        .FirstOrDefault(d => d.OrderID == detailDto.OrderID);
                        if (detail == null)
                        {
                            detail = new OrderDetailEntity();
                            orderToUpdateEntity.Order_Details.Add(detail);
                        }
                        //map first level properties
                        Mapper.Map(detailDto, detail);

                        //update product navigation property
                        var dtoProduct = detailDto.Product;
                        if (dtoProduct != null)
                        {
                            var product = detail.Product ?? new ProductEntity();
                            Mapper.Map(dtoProduct, product);
                        }
                    });
                    _unitOfWork.Commit();
                }
                else
                {
                    throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "Order with the status Shipped and InProgress couldn't be changed" });
                }

            }
            else
            {
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "Order to change was not found in the database" });
            }
        }
    }
}
