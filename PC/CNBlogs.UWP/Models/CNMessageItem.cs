using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.UWP.Models
{
    /// <summary>
    /// 站内信内容（包含每条信息的内容）
    /// </summary>
    class CNMessageItem
    {
        public int Index
        {
            get; set;
        }
        public string MsgID
        {
            get; set;
        }
        public string AuthorName
        {
            get; set;
        }
        public string AuthorHome
        {
            get; set;
        }
        public string AuthorAvatar
        {
            get; set;
        }
        public string Content
        {
            get; set;
        }
        public string Time
        {
            get; set;
        }
        public bool Send
        {
            get; set;
        }
    }
}
