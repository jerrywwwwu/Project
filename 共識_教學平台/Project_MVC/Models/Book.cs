﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class Book
    {
        public int BookId { get; set; }
        public int? UserId { get; set; }
        public int? PostId { get; set; }
        public DateTime BookTime { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}