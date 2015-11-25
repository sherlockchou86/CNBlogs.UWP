using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.UWP.Models
{
    /// <summary>
    /// 站内信（信件概要）
    /// </summary>
    class CNMessage
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
        public bool Inbox
        {
            get; set;
        }
        public string FromOrTo
        {
            get; set;
        }
        public string AuthorID  //该ID不是blog_app
        {
            get; set;
        }
    }
}
