using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Northwind.Classes;
using Northwind.DataLayer;
using Northwind.DataLayer.Repositories;
using Northwind.Helpers;
using Northwind.Infrastructure;
using NorthwindInterfaces;
using NorthwindInterfaces.Exceptions;
using NorthwindInterfaces.Models;
using NorthWind.DataLayer.Infrastructure;

namespace Northwind
{
    [GlobalExceptionHandlerBehaviour(typeof(GlobalExceptionHandler))]
    public class OrderService : IOrderService
    {
        readonly IRepository<OrderEntity> _repository;
        readonly IUnitOfWork _unitOfWork;
        private readonly IOrderDetailRepository _repositoryOrderDetail;
        private IClock _clock;
        private IOrderStateChangeNotifierService orderStateChangeNotifierServiceChannel;

        public OrderService()
        {

        }
        public OrderService(IRepository<OrderEntity> repository, IDataEntityMapper mapper, IUnitOfWork unitOfWork, IOrderDetailRepository repositoryOrderDetail,
            IClock clock
            )
        {
            _repository = repository;
            _repositoryOrderDetail = repositoryOrderDetail;
            _unitOfWork = unitOfWork;
            _clock = clock;

            var servicefactory = new DuplexChannelFactory<IOrderStateChangeNotifierService>("OrderStateChangeNotifierService");
            orderStateChangeNotifierServiceChannel = servicefactory.CreateChannel();
            mapper.CreateMap();
        }
        public IEnumerable<Order> GetOrders()
        {
            var databaseOrders = _repository.GetAll().OrderByDescending(x => x.OrderID).ToList();

            IEnumerable<Order> orders = Mapper.Map<List<OrderEntity>, List<Order>>(databaseOrders);
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
            ValidateEditOrderParameters(orderId, newOrderDto);
            var orderToUpdateEntity = _repository.GetById(orderId);

            if (orderToUpdateEntity != null)
            {
                var orderToUpdate = Mapper.Map<OrderEntity, Order>(orderToUpdateEntity);

                CheckNonEditableFieldsEditAttempt(newOrderDto, orderToUpdate);
                if (orderToUpdate.OrderState == OrderState.New)
                {

                    //update order first level properties
                    Mapper.Map(newOrderDto, orderToUpdateEntity);

                    //delete orderdetails that are not exist in the newOrderDto
                    _repositoryOrderDetail.DeleteOrderDetailsNotInList(newOrderDto.Order_Details.ToList());

                    //update orderdetail
                    newOrderDto.Order_Details.Each(EditOrderDetails);
                    CommitOrderEdit("Error occured during OrderEdit. Please see server logs for details");
                }
                else
                {
                    throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "Order with the status Shipped or InProgress couldn't be changed" });
                }
            }
            else
            {
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "Order to change was not found in the database" });
            }
        }

        private void EditOrderDetails(OrderDetail detailDto)
        {
            var detail = _repositoryOrderDetail.Get(d => d.OrderID == detailDto.OrderID && d.ProductID == detailDto.ProductID);
            //tt
            //detailDto.OrderID = 434343;
            if (detail == null)
            {
                detail = new OrderDetailEntity();
                _repositoryOrderDetail.Add(detail);
            }
            //map first level orderdetail properties
            Mapper.Map(detailDto, detail);
        }

        private void CheckNonEditableFieldsEditAttempt(Order newOrderDto, Order orderToUpdate)
        {
            if (newOrderDto == null) throw new ArgumentNullException("newOrderDto");
            if (orderToUpdate == null) throw new ArgumentNullException("orderToUpdate");

            if (newOrderDto.OrderDate != orderToUpdate.OrderDate ||
                newOrderDto.ShippedDate != orderToUpdate.ShippedDate ||
                newOrderDto.OrderState != orderToUpdate.OrderState)
            {
                throw new FaultException<InvalidOrderChangeException>(
                    new InvalidOrderChangeException()
                    {
                        Message = "OrderID, OrderDate, ShippedDate, OrderState fields couldn't be changed directly"
                    },
                    new FaultReason("OrderID, OrderDate, ShippedDate, OrderState fields couldn't be changed directly"));
            }
        }

        private void ValidateEditOrderParameters(int orderId, Order newOrderDto)
        {
            if (newOrderDto == null)
            {
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "There are no changes received for the order." });
            }

            if (orderId == null)
            {
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "OrderID is empty. Please specify the order to change" });
            }
        }

        public void DeleteOrder(Order orderToDelete)
        {
            if (orderToDelete == null)
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException() { Message = "Order to delete does not specified" });

            if (orderToDelete.OrderState == OrderState.New || orderToDelete.OrderState == OrderState.InWork)
            {
                var id = orderToDelete.OrderID;
                _repositoryOrderDetail.Delete(o => o.OrderID == id);
                _repository.Delete(o => o.OrderID == id);
            }
            else
            {
                throw new FaultException<InvalidOrderChangeException>(
                    new InvalidOrderChangeException() { Message = string.Format("Order is in {0}  state. It cannot be deleted", orderToDelete.OrderState) });
            }

            CommitOrderEdit("Unable to delete the order");

        }

        public void MoveOrderToState(OrderState newState, Order order)
        {
            var now = _clock.Now;
            ValidateMoveStateArgs(newState, order);
            var orderEntity = _repository.GetById(order.OrderID);
            switch (newState)
            {
                case OrderState.InWork: orderEntity.OrderDate = now; break;
                case OrderState.Shipped: orderEntity.ShippedDate = now; break;
            }

            CommitOrderEdit("Error occured during OrderEdit. Please see server logs for details");
            orderStateChangeNotifierServiceChannel.SendOrderStateChange(order.OrderID);
        }

        private void CommitOrderEdit(string exceptionMessage)
        {
            try
            {
                _unitOfWork.Commit();
            }
            catch (DbUpdateException ex)
            {
                Logger.Instance.Error(ex);
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException()
                {
                    Message = exceptionMessage
                });
            }
        }

        private void ValidateMoveStateArgs(OrderState newState, Order order)
        {
            bool shouldChangeState = false;
            if (order.OrderState == newState)
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException()
                {
                    Message = "Orderstate is the same as the current state"
                });
            if (newState == OrderState.New)
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException()
                {
                    Message = "cannot change state to new"
                });
            if (order.OrderState == OrderState.Shipped)
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException()
                {
                    Message = "cannot change state to order with state Shipped"
                });
            if (order.OrderState == OrderState.New && newState == OrderState.Shipped)
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException()
                {
                    Message = "cannot change state from new to shipped. Should be changed to InWork first"
                });
            if (order.OrderState == OrderState.InWork && newState == OrderState.Shipped)
                shouldChangeState = true;
            if (order.OrderState == OrderState.New && newState == OrderState.InWork)
                shouldChangeState = true;
            if (!shouldChangeState)
                throw new FaultException<InvalidOrderChangeException>(new InvalidOrderChangeException()
                {
                    Message = "state change cannot be performed. Please verify the parameters"
                });
        }
    }
}
