﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class ShoppingOrder
    {
        public ShoppingOrder()
        {
            ShoppingOrderDetails = new HashSet<ShoppingOrderDetail>();
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<ShoppingOrderDetail> ShoppingOrderDetails { get; set; }
    }
}