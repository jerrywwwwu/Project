﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class ClassCategory
    {
        public ClassCategory()
        {
            ClassCourses = new HashSet<ClassCourse>();
        }

        public int CourseCategoryId { get; set; }
        public string CourseCategoryName { get; set; }

        public virtual ICollection<ClassCourse> ClassCourses { get; set; }
    }
}