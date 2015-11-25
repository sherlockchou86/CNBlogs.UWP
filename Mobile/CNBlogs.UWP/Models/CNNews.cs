using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.UWP.Models
{
    /// <summary>
    /// 新闻
    /// </summary>
    class CNNews
    {
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
        public string TopicName
        {
            get; set;
        }
        public string TopicIcon
        {
            get; set;
        }
        public string SourceName
        {
            get; set;
        }
        public string NewsRawUrl
        {
            get; set;
        }
    }
}
