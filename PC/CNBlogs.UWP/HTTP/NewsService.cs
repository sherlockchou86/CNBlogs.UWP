using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CNBlogs.UWP.Models;

namespace CNBlogs.UWP.HTTP
{
    /// <summary>
    /// 新闻相关服务
    /// </summary>
    class NewsService
    {
        static string _url_recent_news = "http://wcf.open.cnblogs.com/news/recent/paged/{0}/{1}";  //page_index page_size
        static string _url_recommend_news = "http://wcf.open.cnblogs.com/news/recommend/paged/{0}/{1}";  //page_index page_size
        static string _url_news_content = "http://wcf.open.cnblogs.com/news/item/{0}"; //news_id
        static string _url_news_comment = "http://wcf.open.cnblogs.com/news/item/{0}/comments/{1}/{2}";  //news_id page_index page_size

        /// <summary>
        /// 获取首页新闻
        /// </summary>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNNews>> GetRecentNewsAsync(int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_recent_news, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);

                if (xml != null)
                {
                    List<CNNews> list_news = new List<CNNews>();
                    CNNews news;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNode feed = doc.ChildNodes[1];

                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            news = new CNNews();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    news.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    news.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    news.Summary = node2.InnerText;
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    news.PublishTime = t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    news.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    news.NewsRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    news.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    news.Views = node2.InnerText;
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    news.Comments = node2.InnerText;
                                }
                                if (node2.Name.Equals("topic"))
                                {
                                    if (node2.HasChildNodes)
                                    {
                                        news.TopicName = node2.InnerText;
                                    }
                                    else
                                    {
                                        news.TopicName = "";
                                    }
                                }
                                if (node2.Name.Equals("topicIcon"))
                                {
                                    if (node2.HasChildNodes)
                                    {
                                        news.TopicIcon = node2.InnerText;
                                    }
                                    else
                                    {
                                        news.TopicIcon = "http://static.cnblogs.com/images/logo_small.gif";
                                    }
                                }
                                if (node2.Name.Equals("sourceName"))
                                {
                                    news.SourceName = node2.InnerText;
                                }
                            }
                            list_news.Add(news);
                        }
                    }
                    return list_news;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取指定新闻内容
        /// </summary>
        /// <param name="news_id"></param>
        /// <returns></returns>
        public async static Task<string> GetNewsContentAsync(string news_id)
        {
            try
            {
                string url = string.Format(_url_news_content, news_id);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    if (doc.ChildNodes.Count == 2 && doc.ChildNodes[1].ChildNodes[3].Name.Equals("Content"))
                    {
                        return "<style>body{font-family:微软雅黑;font-size=14px}</style>" + doc.ChildNodes[1].ChildNodes[3].InnerText;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取指定新闻评论
        /// </summary>
        /// <param name="news_id"></param>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNNewsComment>> GetNewsCommentsAysnc(string news_id,int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_news_comment, news_id, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    List<CNNewsComment> list_comments = new List<CNNewsComment>();
                    CNNewsComment comment;
                    XmlNode feed = doc.ChildNodes[1];

                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            comment = new CNNewsComment();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    comment.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    comment.PublishTime = t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    comment.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    comment.AuthorName = node2.ChildNodes[0].InnerText;
                                    comment.AuthorHome = node2.ChildNodes[1].InnerText;
                                    comment.AuthorAvatar = "http://pic.cnblogs.com/avatar/simple_avatar.gif";  //api中没有头像url
                                }
                                if (node2.Name.Equals("content"))
                                {
                                    comment.Content = node2.InnerText;
                                }
                            }
                            list_comments.Add(comment);
                        }
                    }
                    return list_comments;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取推荐新闻
        /// </summary>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNNews>> GetTopDiggsAsync(int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_recommend_news, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);

                if (xml != null)
                {
                    List<CNNews> list_news = new List<CNNews>();
                    CNNews news;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNode feed = doc.ChildNodes[1];

                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            news = new CNNews();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    news.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    news.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    news.Summary = node2.InnerText;
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    news.PublishTime = t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    news.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    news.NewsRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    news.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    news.Views = node2.InnerText;
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    news.Comments = node2.InnerText;
                                }
                                if (node2.Name.Equals("topic"))
                                {
                                    if (node2.HasChildNodes)
                                    {
                                        news.TopicName = node2.InnerText;
                                    }
                                    else
                                    {
                                        news.TopicName = "";
                                    }
                                }
                                if (node2.Name.Equals("topicIcon"))
                                {
                                    if (node2.HasChildNodes)
                                    {
                                        news.TopicIcon = node2.InnerText;
                                    }
                                    else
                                    {
                                        news.TopicIcon = "http://static.cnblogs.com/images/logo_small.gif";
                                    }
                                }
                                if (node2.Name.Equals("sourceName"))
                                {
                                    news.SourceName = node2.InnerText;
                                }
                            }
                            list_news.Add(news);
                        }
                    }
                    return list_news;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
