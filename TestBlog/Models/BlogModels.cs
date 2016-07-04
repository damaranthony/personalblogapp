using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using BlogData.Data;

namespace TestBlog.Models
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ContentStateType State { get; set; }
        public string Author { get; set; }

        [DisplayName("Publish Date")]
        public DateTime PublishDate { get; set; }
        public string Comment { get; set; }

    }
}