﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Playground.Mvc.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SeraphEntities : DbContext
    {
        public SeraphEntities()
            : base("name=SeraphEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<APP_USER> APP_USER { get; set; }
        public virtual DbSet<APP_USER_ROLE> APP_USER_ROLE { get; set; }
        public virtual DbSet<BOOKMARK> BOOKMARKs { get; set; }
        public virtual DbSet<BOOKMARK_CATEGORY> BOOKMARK_CATEGORY { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<EMPLOYEE> EMPLOYEEs { get; set; }
        public virtual DbSet<EMPLOYEE_PHOTO> EMPLOYEE_PHOTO { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
    }
}