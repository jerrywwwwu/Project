﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class ClassChapter
    {
        public int ChapterId { get; set; }
        public int? CourseId { get; set; }
        public string CourseVideo { get; set; }
        public string ChapterTitle { get; set; }

        public virtual ClassCourse Course { get; set; }
    }
}