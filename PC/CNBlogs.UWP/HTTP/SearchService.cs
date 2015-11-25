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
    /// 搜索服务
    /// </summary>
    class SearchService
    {
        static string _url_search_bloger = "http://wcf.open.cnblogs.com/blog/bloggers/search?t={0}"; //blogger_keywords
        static string _url_search_blogs = "http://zzk.cnblogs.com/s?w={0}&t=b&p={1}";  //blog_keywords page_index

        /// <summary>
        /// 搜索博客
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="page_index"></param>
        /// <returns></returns>
        public async static Task<List<CNBlog>> SearchBlogs(string keywords,int page_index)
        {
            try
            {
                string url = string.Format(_url_search_blogs, keywords, page_index);
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    html = html.Split(new string[] { "<div class=\"forflow\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "<div class=\"forflow\" id=\"sidebar\">" },StringSplitOptions.None)[0]
                        .Split(new string[] {"<div id=\"paging_block\""},StringSplitOptions.None)[0];
                    html = "<?xml version=\"1.0\" encoding=\"utf - 8\" ?> " + "<result>" + html + "</result>";
                    List<CNBlog> list_blogs = new List<CNBlog>();
                    CNBlog blog;

                    XmlDocument doc = new XmlDocument();
                    
                    doc.LoadXml(html);

                    XmlNode search_items = doc.ChildNodes[1];
                    if (search_items != null)
                    {
                        foreach (XmlNode node in search_items.ChildNodes)
                        {
                            blog = new CNBlog();
                            blog.Title = node.ChildNodes[0].InnerText;
                            blog.Summary = node.ChildNodes[2].InnerText;
                            blog.AuthorName = node.ChildNodes[4].ChildNodes[0].InnerText;
                            blog.AuthorHome = node.ChildNodes[4].ChildNodes[0].ChildNodes[0].Attributes["href"].Value;
                            blog.BlogApp = blog.AuthorHome.Split('/')[3];
                            blog.PublishTime = node.ChildNodes[4].ChildNodes[1].InnerText;
                            if(node.ChildNodes[4].ChildNodes[2]!=null)
                            {
                                if(node.ChildNodes[4].ChildNodes[2].InnerText.Contains("推荐"))
                                {
                                    blog.Diggs = node.ChildNodes[4].ChildNodes[2].InnerText.Split('(')[1].TrimEnd(')');
                                }
                                if (node.ChildNodes[4].ChildNodes[2].InnerText.Contains("评论"))
                                {
                                    blog.Comments = "[" + node.ChildNodes[4].ChildNodes[2].InnerText.Split('(')[1].TrimEnd(')') + "]";
                                }
                                if(node.ChildNodes[4].ChildNodes[2].InnerText.Contains("浏览"))
                                {
                                    blog.Views = "[" + node.ChildNodes[4].ChildNodes[2].InnerText.Split('(')[1].TrimEnd(')') + "]";
                                }
                            }
                            if (node.ChildNodes[4].ChildNodes[3] != null)
                            {
                                if (node.ChildNodes[4].ChildNodes[3].InnerText.Contains("推荐"))
                                {
                                    blog.Diggs = node.ChildNodes[4].ChildNodes[3].InnerText.Split('(')[1].TrimEnd(')');
                                }
                                if (node.ChildNodes[4].ChildNodes[3].InnerText.Contains("评论"))
                                {
                                    blog.Comments = "[" + node.ChildNodes[4].ChildNodes[3].InnerText.Split('(')[1].TrimEnd(')') + "]";
                                }
                                if (node.ChildNodes[4].ChildNodes[3].InnerText.Contains("浏览"))
                                {
                                    blog.Views = "[" + node.ChildNodes[4].ChildNodes[3].InnerText.Split('(')[1].TrimEnd(')') + "]";
                                }
                            }
                            if (node.ChildNodes[4].ChildNodes[4] != null)
                            {
                                if (node.ChildNodes[4].ChildNodes[4].InnerText.Contains("推荐"))
                                {
                                    blog.Diggs = node.ChildNodes[4].ChildNodes[4].InnerText.Split('(')[1].TrimEnd(')');
                                }
                                if (node.ChildNodes[4].ChildNodes[4].InnerText.Contains("评论"))
                                {
                                    blog.Comments = "[" + node.ChildNodes[4].ChildNodes[4].InnerText.Split('(')[1].TrimEnd(')') + "]";
                                }
                                if (node.ChildNodes[4].ChildNodes[4].InnerText.Contains("浏览"))
                                {
                                    blog.Views = "[" + node.ChildNodes[4].ChildNodes[4].InnerText.Split('(')[1].TrimEnd(')') + "]";
                                }
                            }
                            blog.BlogRawUrl = node.ChildNodes[5].InnerText;
                            blog.AuthorAvator = "http://pic.cnblogs.com/avatar/simple_avatar.gif";

                            string[] strs = blog.BlogRawUrl.Split('/');
                            blog.ID = strs[strs.Length - 1].Split('.')[0];

                            if (blog.Diggs == null)
                            {
                                blog.Diggs = "0";
                            }
                            if (blog.Comments == null)
                            {
                                blog.Comments = "[0]";
                            }
                            list_blogs.Add(blog);
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
        /// 搜索博主
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public async static Task<List<CNBloger>> SearchBloger(string keywords)
        {
            try
            {
                string url = string.Format(_url_search_bloger, keywords);
                string xml = await BaseService.SendGetRequest(url);

                if (xml != null)
                {
                    List<CNBloger> list_bloger = new List<CNBloger>();
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
                                if (node2.Name.Equals("title"))
                                {
                                    bloger.BlogerName = node2.InnerText;
                                }
                                if (node2.Name.Equals("updated"))
                                {
                                    DateTime t = DateTime.Parse(node2.InnerText);
                                    bloger.UpdateTime = "最后更新 " + t.ToString();
                                }
                                if (node2.Name.Equals("blogapp"))
                                {
                                    bloger.BlogApp = node2.InnerText;
                                }
                                if (node2.Name.Equals("avatar"))
                                {
                                    bloger.BlogerAvator = node2.InnerText.Equals("") ? "http://pic.cnblogs.com/avatar/simple_avatar.gif" : node2.InnerText;
                                }
                                if (node2.Name.Equals("postcount"))
                                {
                                    bloger.PostCount = "发表博客 " + node2.InnerText + "篇";
                                }
                            }
                            list_bloger.Add(bloger);
                        }
                    }
                    return list_bloger;
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
