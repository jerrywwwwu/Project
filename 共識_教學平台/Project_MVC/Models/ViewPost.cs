﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Project_MVC.Models
{
    public partial class ViewPost
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostCategory { get; set; }
        public string PostContent { get; set; }
        public DateTime PostPublishTime { get; set; }
        public string PostrImgUrl { get; set; }
        public string PostTag1 { get; set; }
        public string PostTag2 { get; set; }
        public string PostTag3 { get; set; }
        public string HiddenPost { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public int? PostThumbCount { get; set; }
        public int? PostCommentCount { get; set; }
        public int? PostBookCount { get; set; }
        public int? ValuePoint { get; set; }
    }
}