using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NorthWind.DataLayer.Infrastructure;
using NorthwindInterfaces.Models;

namespace Northwind.DataLayer.Repositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetailEntity>
    {
        IEnumerable<OrderDetailEntity> GetOrderDetailsNotInList(List<OrderDetail> filterList);
    }

    public class OrderDetailRepository : RepositoryBase<OrderDetailEntity>, IOrderDetailRepository
    {
        public OrderDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
             
        }

        public IEnumerable<OrderDetailEntity> GetOrderDetailsNotInList(List<OrderDetail> filterList)
        {
            var query = DataContext.Order_Details.AsQueryable();
            // OrderDetails.Where(orderDetailOrm => (orderDetailOrm.OrderId == 11000) && !(orderDetailOrm.ProductId == 4 || orderDetailOrm.ProductId == 25))

            // ReSharper disable once RedundantAssignment
            Expression predicateBody = Expression.Constant(true);
            ParameterExpression pe = Expression.Parameter(typeof(OrderDetailEntity), "orderDetailOrm");

            var tt = filterList.Select(x => x.OrderID);
            var uniqueOrderId = new HashSet<int>(tt).FirstOrDefault();
            if (uniqueOrderId == 0)
            {
                throw new FormatException ("OrderId cannot be 0");
            }
            //(orderDetailOrm.OrderId == 11000)
            Expression left = Expression.Property(pe, typeof(OrderDetailEntity).GetProperty("OrderID"));
            Expression right = Expression.Constant(uniqueOrderId);
            Expression orderClauseBody = Expression.Equal(left, right);

            //!(orderDetailOrm.ProductId == 4 || orderDetailOrm.ProductId == 25)
            Expression productClauseBody = Expression.Constant(true);
            var first = filterList.First();
            foreach (var filter in filterList)
            {
                left = Expression.Property(pe, typeof(OrderDetailEntity).GetProperty("ProductID"));
                right = Expression.Constant(filter.ProductID);
                Expression e2 = Expression.Equal(left, right);
                productClauseBody = first == filter ? e2 : Expression.AndAlso(productClauseBody, e2);
            }
            productClauseBody = Expression.Not(productClauseBody);

            predicateBody = Expression.AndAlso(orderClauseBody,productClauseBody);

            var whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { query.ElementType },
                query.Expression,
                Expression.Lambda<Func<OrderDetailEntity, bool>>(predicateBody, pe)
                );

            var results = query.Provider.CreateQuery<OrderDetailEntity>(whereCallExpression);
            return results.ToList();
        }
    }
}
