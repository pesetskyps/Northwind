using AutoMapper;
using Northwind.DataLayer;
using Northwind.Resolvers;
using NorthwindInterfaces.Models;

namespace Northwind.Classes
{
    public interface IDataEntityMapper
    {
        void CreateMap();
    }
    public class DataEntityMapper : IDataEntityMapper
    {
        public void CreateMap()
        {
            Mapper.CreateMap<OrderEntity, Order>()
                .ForMember(dest => dest.OrderState, opt => opt.ResolveUsing<OrderStateResolver>());
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<OrderDetailEntity, OrderDetail>();
            Mapper.CreateMap<CustomerEntity, Customer>();

            //ORM to DB
            Mapper.CreateMap<Order, OrderEntity>()
                .ForMember(dest => dest.Order_Details, mo => mo.Ignore())
                .ForMember(dest => dest.Customer, mo => mo.Ignore())
                .ForMember(dest => dest.Employee, mo => mo.Ignore())
                .ForMember(dest => dest.Shipper, mo => mo.Ignore())
                .ForMember(dest => dest.OrderID, mo => mo.Ignore())
                .ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));

            Mapper.CreateMap<OrderDetail, OrderDetailEntity>()
                .ForMember(dest => dest.Product, mo => mo.Ignore())
                .ForMember(dest => dest.Order, mo => mo.Ignore())
                .ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));


            Mapper.CreateMap<Product, ProductEntity>()
                .ForMember(dest => dest.Category, mo => mo.Ignore())
                .ForMember(dest => dest.Order_Details, mo => mo.Ignore())
                .ForMember(dest => dest.Supplier, mo => mo.Ignore())
                .ForMember(dest => dest.ProductID, mo => mo.Ignore())
                .ForAllMembers(opt => opt.Condition(srs => !srs.IsSourceValueNull));
        }
    }
}
