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
    /// 博客相关服务
    /// </summary>
    static class BlogService
    {
        static string _url_recent_blog = "http://wcf.open.cnblogs.com/blog/sitehome/paged/{0}/{1}"; //page_index page_size
        static string _url_48_views = "http://wcf.open.cnblogs.com/blog/48HoursTopViewPosts/{0}";  //item_count
        static string _url_10_diggs = "http://wcf.open.cnblogs.com/blog/TenDaysTopDiggPosts/{0}";  //item_count
        static string _url_user_blog = "http://wcf.open.cnblogs.com/blog/u/{0}/posts/{1}/{2}";  //blog_app page_index page_size
        static string _url_recommend_bloger = "http://wcf.open.cnblogs.com/blog/bloggers/recommend/{0}/{1}";  //page_index page_size
        static string _url_blog_content = "http://wcf.open.cnblogs.com/blog/post/body/{0}";  //post_id
        static string _url_blog_comment = "http://wcf.open.cnblogs.com/blog/post/{0}/comments/{1}/{2}";  //post_id page_index page_size

        /// <summary>
        /// 分页获取首页博客
        /// </summary>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNBlog>> GetRecentBlogsAsync(int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_recent_blog, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog cnblog;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            cnblog = new CNBlog();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    cnblog.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    cnblog.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    cnblog.Summary = node2.InnerText + "...";
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    cnblog.PublishTime = "发表于 " + t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    cnblog.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    cnblog.AuthorName = node2.ChildNodes[0].InnerText;
                                    cnblog.AuthorHome = node2.ChildNodes[1].InnerText;
                                    cnblog.AuthorAvator = node2.ChildNodes[2].InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : node2.ChildNodes[2].InnerText;
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    cnblog.BlogRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("blogapp"))
                                {
                                    cnblog.BlogApp = node2.InnerText;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    cnblog.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    cnblog.Views = "["+node2.InnerText+"]";
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    cnblog.Comments = "["+node2.InnerText+"]";
                                }
                            }
                            list_blogs.Add(cnblog);
                        }
                    }
                    return list_blogs;
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
        /// 获取指定博客正文
        /// </summary>
        /// <param name="post_id"></param>
        /// <returns></returns>
        public async static Task<string> GetBlogContentAsync(string post_id)
        {
            try
            {
                string url = string.Format(_url_blog_content, post_id);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    if (doc.ChildNodes.Count == 2 && doc.ChildNodes[1].Name.Equals("string"))
                    {
                        return "<style>body{font-family:微软雅黑;font-size=14px}</style>" + doc.ChildNodes[1].InnerText;
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
        /// 根据博主blog_app获取博客列表
        /// </summary>
        /// <param name="blog_app"></param>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNBlog>> GetBlogsByUserAsync(string blog_app,int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_user_blog, blog_app, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog cnblog;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            cnblog = new CNBlog();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    cnblog.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    cnblog.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    cnblog.Summary = node2.InnerText + "...";
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    cnblog.PublishTime = "发表于 " + t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    cnblog.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    cnblog.AuthorName = node2.ChildNodes[0].InnerText;
                                    cnblog.AuthorHome = node2.ChildNodes[1].InnerText;
                                    cnblog.AuthorAvator = doc.ChildNodes[1].ChildNodes[3].InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : doc.ChildNodes[1].ChildNodes[3].InnerText;
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    cnblog.BlogRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    cnblog.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    cnblog.Views = "[" + node2.InnerText + "]";
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    cnblog.Comments = "[" + node2.InnerText + "]";
                                }
                            }
                            cnblog.BlogApp = cnblog.AuthorHome.Split('/')[3];
                            list_blogs.Add(cnblog);
                        }
                    }
                    return list_blogs;
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
        /// 根据博客id获取评论
        /// </summary>
        /// <param name="post_id"></param>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNBlogComment>> GetBlogCommentsAsync(string post_id,int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_blog_comment, post_id, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    List<CNBlogComment> list_comments = new List<CNBlogComment>();
                    CNBlogComment comment;
                    XmlNode feed = doc.ChildNodes[1];

                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            comment = new CNBlogComment();
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
        /// 获取48小时阅读排行榜
        /// </summary>
        /// <param name="item_count"></param>
        /// <returns></returns>
        public async static Task<List<CNBlog>> Get48TopViewsAysnc(int item_count)
        {
            try
            {
                string url = string.Format(_url_48_views, item_count);
                string xml = await BaseService.SendGetRequest(url);
                if(xml != null)
                {
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog cnblog;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            cnblog = new CNBlog();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    cnblog.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    cnblog.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    cnblog.Summary = node2.InnerText + "...";
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    cnblog.PublishTime = "发表于 " + t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    cnblog.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    cnblog.AuthorName = node2.ChildNodes[0].InnerText;
                                    cnblog.AuthorHome = node2.ChildNodes[1].InnerText;
                                    if (node2.ChildNodes.Count == 3)
                                    {
                                        cnblog.AuthorAvator = node2.ChildNodes[2].InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : node2.ChildNodes[2].InnerText;
                                    }
                                    else
                                    {
                                        cnblog.AuthorAvator = "http://pic.cnblogs.com/avatar/simple_avatar.gif";
                                    }
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    cnblog.BlogRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    cnblog.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    cnblog.Views = "[" + node2.InnerText + "]";
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    cnblog.Comments = "[" + node2.InnerText + "]";
                                }
                            }
                            cnblog.BlogApp = cnblog.AuthorHome.Split('/')[3];
                            list_blogs.Add(cnblog);
                        }
                    }
                    return list_blogs;
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
        /// 十天推荐榜
        /// </summary>
        /// <param name="item_count"></param>
        /// <returns></returns>
        public async static Task<List<CNBlog>> Get10TopDiggsAysnc(int item_count)
        {
            try
            {
                string url = string.Format(_url_10_diggs, item_count);
                string xml = await BaseService.SendGetRequest(url);

                if (xml != null)
                {
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog cnblog;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            cnblog = new CNBlog();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    cnblog.ID = node2.InnerText;
                                }
                                if (node2.Name.Equals("title"))
                                {
                                    cnblog.Title = node2.InnerText;
                                }
                                if (node2.Name.Equals("summary"))
                                {
                                    cnblog.Summary = node2.InnerText + "...";
                                }
                                if (node2.Name.Equals("published"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    cnblog.PublishTime = "发表于 " + t.ToString();
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    cnblog.UpdateTime = node2.InnerText;
                                }
                                if (node2.Name.Equals("author"))
                                {
                                    cnblog.AuthorName = node2.ChildNodes[0].InnerText;
                                    cnblog.AuthorHome = node2.ChildNodes[1].InnerText;
                                    if (node2.ChildNodes.Count == 3)
                                    {
                                        cnblog.AuthorAvator = node2.ChildNodes[2].InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : node2.ChildNodes[2].InnerText;
                                    }
                                    else
                                    {
                                        cnblog.AuthorAvator = "http://pic.cnblogs.com/avatar/simple_avatar.gif";
                                    }
                                }
                                if (node2.Name.Equals("link"))
                                {
                                    cnblog.BlogRawUrl = node2.Attributes["href"].Value;
                                }
                                if (node2.Name.Equals("diggs"))
                                {
                                    cnblog.Diggs = node2.InnerText;
                                }
                                if (node2.Name.Equals("views"))
                                {
                                    cnblog.Views = "[" + node2.InnerText + "]";
                                }
                                if (node2.Name.Equals("comments"))
                                {
                                    cnblog.Comments = "[" + node2.InnerText + "]";
                                }
                            }
                            cnblog.BlogApp = cnblog.AuthorHome.Split('/')[3];
                            list_blogs.Add(cnblog);
                        }
                    }
                    return list_blogs;
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
        /// 获取推荐博主
        /// </summary>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        public async static Task<List<CNBloger>> GetTopDiggBlogers(int page_index,int page_size)
        {
            try
            {
                string url = string.Format(_url_recommend_bloger, page_index, page_size);
                string xml = await BaseService.SendGetRequest(url);
                if (xml != null)
                {
                    List<CNBloger> list_blogers = new List<CNBloger>();
                    CNBloger bloger;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNode feed = doc.ChildNodes[1];
                    foreach (XmlNode node in feed.ChildNodes)
                    {
                        if (node.Name.Equals("entry"))
                        {
                            bloger = new CNBloger();
                            foreach (XmlNode node2 in node.ChildNodes)
                            {
                                if (node2.Name.Equals("id"))
                                {
                                    bloger.BlogerHome = node2.InnerText;
                                }
                                if(node2.Name.Equals("title"))
                                {
                                    bloger.BlogerName = node2.InnerText;
                                }
                                if(node2.Name.Equals("updated"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    bloger.UpdateTime = "最后更新 " + t.ToString();
                                }
                                if(node2.Name.Equals("blogapp"))
                                {
                                    bloger.BlogApp = node2.InnerText;
                                }
                                if(node2.Name.Equals("avatar"))
                                {
                                    bloger.BlogerAvator = node2.InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : node2.InnerText;
                                }
                                if(node2.Name.Equals("postcount"))
                                {
                                    bloger.PostCount = "发表博客 " + node2.InnerText + "篇";
                                }
                            }
                            list_blogers.Add(bloger);
                        }
                    }
                    return list_blogers;
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
