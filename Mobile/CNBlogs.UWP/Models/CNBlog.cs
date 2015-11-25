using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.UWP.Models
{
    /// <summary>
    /// 博客
    /// </summary>
    class CNBlog
    {
        public int Index
        {
            get; set;
        }
        public string ID
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
        public string Summary
        {
            get; set;
        }
        public string PublishTime
        {
            get; set;
        }
        public string UpdateTime
        {
            get; set;
        }
        public string AuthorName
        {
            get; set;
        }
        public string AuthorAvator
        {
            get; set;
        }
        public string AuthorHome
        {
            get; set;
        }
        public string BlogRawUrl
        {
            get; set;
        }
        public string BlogApp
        {
            get; set;
        }
        public string Diggs
        {
            get; set;
        }
        public string Views
        {
            get; set;
        }
        public string Comments
        {
            get; set;
        }
    }
}
