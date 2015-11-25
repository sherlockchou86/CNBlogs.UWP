using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.UWP.Models
{
    /// <summary>
    /// 收藏
    /// </summary>
    class CNCollection
    { 
        public string Title
        {
            get; set;
        }
        public string RawUrl
        {
            get; set;
        }
        public string Summary
        {
            get; set;
        }
        public string CollectionTime
        {
            get; set;
        }
        public string CollectionCount
        {
            get; set;
        }
        public string Category
        {
            get; set;
        }

    }
}
