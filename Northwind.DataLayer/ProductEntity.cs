//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ProductEntity
    {
        public ProductEntity()
        {
            this.Order_Details = new HashSet<OrderDetailEntity>();
        }
    
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<short> UnitsInStock { get; set; }
        public Nullable<short> UnitsOnOrder { get; set; }
        public Nullable<short> ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    
        public virtual CategoryEntity Category { get; set; }
        public virtual ICollection<OrderDetailEntity> Order_Details { get; set; }
        public virtual SupplierEntity Supplier { get; set; }
    }
}
