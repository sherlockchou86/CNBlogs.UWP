using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.UWP.Models
{
    /// <summary>
    /// 博客园用户 注意与CNBloger不同
    /// </summary>
    class CNUserInfo
    {
        public string BlogApp
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Avatar
        {
            get; set;
        }
        public string Age  //园龄
        {
            get; set;
        }
        public string Followers
        {
            get; set;
        }
        public string Followees
        {
            get; set;
        }
        public string BlogHome
        {
            get; set;
        }
        public string GUID  //用户唯一标示
        {
            get; set;
        }
    }
}
