﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class User
    {
        public User()
        {
            Books = new HashSet<Book>();
            ChatRooms = new HashSet<ChatRoom>();
            ClassCourses = new HashSet<ClassCourse>();
            ClassMessages = new HashSet<ClassMessage>();
            ClassQas = new HashSet<ClassQa>();
            ClassScores = new HashSet<ClassScore>();
            ClassTexts = new HashSet<ClassText>();
            Comments = new HashSet<Comment>();
            Messagesses = new HashSet<Messagess>();
            Posts = new HashSet<Post>();
            ShoppingAddToCarts = new HashSet<ShoppingAddToCart>();
            ShoppingOrders = new HashSet<ShoppingOrder>();
            Thumbs = new HashSet<Thumb>();
            UserTeachers = new HashSet<UserTeacher>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserGender { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public DateTime UserCreateTime { get; set; }
        public string UserImageUrl { get; set; }

        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<ChatRoom> ChatRooms { get; set; }
        public virtual ICollection<ClassCourse> ClassCourses { get; set; }
        public virtual ICollection<ClassMessage> ClassMessages { get; set; }
        public virtual ICollection<ClassQa> ClassQas { get; set; }
        public virtual ICollection<ClassScore> ClassScores { get; set; }
        public virtual ICollection<ClassText> ClassTexts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Messagess> Messagesses { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<ShoppingAddToCart> ShoppingAddToCarts { get; set; }
        public virtual ICollection<ShoppingOrder> ShoppingOrders { get; set; }
        public virtual ICollection<Thumb> Thumbs { get; set; }
        public virtual ICollection<UserTeacher> UserTeachers { get; set; }
    }
}