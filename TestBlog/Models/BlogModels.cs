using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestBlog.Models
{
    public class BlogModels
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}