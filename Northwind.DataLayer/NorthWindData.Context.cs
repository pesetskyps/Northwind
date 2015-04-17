﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Northwind.DataLayer
{
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class NorthwindData : DbContext
    {
        public NorthwindData()
            : base("name=NorthwindData")
        {
        }

        public NorthwindData(DbConnection connection)
            : base(connection, true)
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<CustomerDemographicEntity> CustomerDemographicEntities { get; set; }
        public DbSet<CustomerEntity> CustomerEntities { get; set; }
        public DbSet<EmployeeEntity> EmployeeEntities { get; set; }
        public DbSet<OrderDetailEntity> Order_Details { get; set; }
        public DbSet<OrderEntity> OrderEntities { get; set; }
        public DbSet<ProductEntity> ProductEntities { get; set; }
        public DbSet<RegionEntity> RegionEntities { get; set; }
        public DbSet<ShipperEntity> ShipperEntities { get; set; }
        public DbSet<SupplierEntity> SupplierEntities { get; set; }
        public DbSet<TerritoryEntity> TerritoryEntities { get; set; }
    }
}
