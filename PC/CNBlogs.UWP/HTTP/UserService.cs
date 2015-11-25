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
    /// 用户相关服务（所有需要登录的操作均由该服务负责）(该类方法执行的前提是：已经登录)
    /// </summary>
    static class UserService
    {
        private static string _url_current_user_info = "http://home.cnblogs.com/ajax/user/CurrentIngUserInfo";
        private static string _url_user_info = "http://home.cnblogs.com/u/{0}/";
        private static string _url_user_followers = "http://home.cnblogs.com/u/{0}/followers/{1}/";
        private static string _url_user_followees = "http://home.cnblogs.com/u/{0}/followees/{1}/";

        private static string _url_inbox_messages = "http://msg.cnblogs.com/mobile/inbox/{0}";
        private static string _url_outbox_messages = "http://msg.cnblogs.com/mobile/outbox/{0}";
        private static string _url_messages_items = "http://msg.cnblogs.com/mobile/item/{0}";

        private static string _url_collections = "http://wz.cnblogs.com/my/{0}.html";

        private static string _url_digg_blog = "http://www.cnblogs.com/mvc/vote/VoteBlogPost.aspx";

        private static string _url_digg_news = "http://news.cnblogs.com/News/VoteNews";

        private static string _url_follow_user = "http://home.cnblogs.com/ajax/follow/FollowUser";

        private static string _url_sendmsg = "http://msg.cnblogs.com/ajax/msg/send";
        private static string _url_replymsg = "http://msg.cnblogs.com/ajax/msg/reply";

        private static string _url_add_blog_comment = "http://www.cnblogs.com/mvc/PostComment/Add.aspx";
        private static string _url_add_news_comment = "http://news.cnblogs.com/Comment/InsertComment";

        private static string _url_get_wztags = "http://wz.cnblogs.com/ajax/wz/GetUserTags";
        private static string _url_add_wz = "http://wz.cnblogs.com/ajax/wz/AddWzlink";
        /// <summary>
        /// 获取当前登录用户的信息
        /// </summary>
        /// <returns></returns>
        public async static Task<CNUserInfo> GetCurrentUserInfo()
        {
            try
            {
                string html = await BaseService.SendPostRequest(_url_current_user_info, "");
                if (html != null)
                {
                    CNUserInfo user = new CNUserInfo();
                    user.Avatar = html.Split(new string[] { "src=\\\"" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\\\" alt" }, StringSplitOptions.None)[0];

                    user.BlogApp = html.Split(new string[] { "href=\\\"/u/" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "/\\\" class=\\\"big bold\\\"" }, StringSplitOptions.None)[0];

                    user.Name = html.Split(new string[] { "class=\\\"big bold\\\"\\u003e" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\\" }, StringSplitOptions.None)[0];

                    return user;
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
        /// 获取指定用户信息
        /// </summary>
        /// <param name="blog_app"></param>
        /// <returns></returns>
        public async static Task<CNUserInfo> GetUserInfo(string blog_app)
        {
            try
            {
                string url = string.Format(_url_user_info, blog_app);
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    CNUserInfo user = new CNUserInfo();

                    user.GUID = html.Split(new string[] { "var currentUserId = " }, StringSplitOptions.None)[1]
                        .Split(new string[] { "var isLogined = true;" }, StringSplitOptions.None)[0].Trim().Trim(';');

                    string avatar = html.Split(new string[] { "<div class=\"user_avatar\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "class=\"img_avatar\">" }, StringSplitOptions.None)[0]
                        .Split(new string[] { "src=\"" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "\" alt=" }, StringSplitOptions.None)[0];

                    user.Avatar = avatar;

                    html = html.Split(new string[] { "<td valign=\"top\">" }, StringSplitOptions.None)[2]
                        .Split(new string[] { "<div class=\"user_intro\">" }, StringSplitOptions.None)[0]
                        .Split(new string[] { "<br>" }, StringSplitOptions.None)[0];

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + "<result>" + html + "</result>";

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    user.Name = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerText.Trim();
                    XmlNodeList lis = doc.GetElementsByTagName("li");
                    foreach (XmlNode n in lis)
                    {
                        if (n.ChildNodes.Count == 2)
                        {
                            if (n.ChildNodes[0].InnerText.Contains("园龄"))
                            {
                                user.Age = "园龄 " + n.ChildNodes[1].InnerText;
                            }
                            if (n.ChildNodes[0].InnerText.Contains("博客"))
                            {
                                user.BlogHome = n.ChildNodes[1].InnerText;
                            }
                        }
                    }

                    user.Followees = "关注" + doc.ChildNodes[1].ChildNodes[0].ChildNodes[2].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].InnerText;

                    user.Followers = "粉丝" + doc.ChildNodes[1].ChildNodes[0].ChildNodes[2].ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].InnerText;

                    user.BlogApp = user.BlogHome.Split('/')[3];

                    return user;
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
        /// 获取指定用户粉丝
        /// </summary>
        /// <param name="blog_app"></param>
        /// <param name="page_index"></param>
        /// <returns></returns>
        public async static Task<List<CNUserInfo>> GetFollowers(string blog_app,int page_index)
        {
            try
            {
                string url = string.Format(_url_user_followers, blog_app, page_index) + "?t=" + DateTime.Now.Millisecond;
                string html = await BaseService.SendGetRequest(url);
                if (html != null)
                {
                    List<CNUserInfo> list_followers = new List<CNUserInfo>();
                    CNUserInfo user;

                    html = html.Split(new string[] { "<div class=\"avatar_list\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "<div class=\"clear\">" }, StringSplitOptions.None)[0];
                    html = "<div>" + html.Replace("&amp;", "").Replace("&","");

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + html.Replace("\"></a>", "\"/></a>");

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    XmlNode ul = doc.ChildNodes[1].ChildNodes[0];

                    foreach (XmlNode node in ul.ChildNodes)
                    {
                        user = new CNUserInfo();

                        user.Avatar = node.ChildNodes[0].ChildNodes[0].ChildNodes[0].Attributes["src"].Value;
                        user.Name = node.ChildNodes[1].ChildNodes[0].InnerText;
                        user.BlogApp = node.ChildNodes[1].ChildNodes[0].Attributes["href"].Value.Split('/')[2];

                        list_followers.Add(user);
                    }

                    return list_followers;
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
        /// 获取指定用户的关注
        /// </summary>
        /// <param name="blog_app"></param>
        /// <param name="page_index"></param>
        /// <returns></returns>
        public async static Task<List<CNUserInfo>> GetFollowees(string blog_app,int page_index)
        {
            try
            {
                string url = string.Format(_url_user_followees, blog_app, page_index) + "?t=" + DateTime.Now.Millisecond;
                string html = await BaseService.SendGetRequest(url);
                if (html != null)
                {
                    List<CNUserInfo> list_followees = new List<CNUserInfo>();
                    CNUserInfo user;

                    html = html.Split(new string[] { "<div class=\"avatar_list\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "<div class=\"clear\"></div>" }, StringSplitOptions.None)[0];
                    html = "<div>" + html.Replace("&amp;", "").Replace("&","");

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + html.Replace("\"></a>", "\"/></a>");

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    XmlNode ul = doc.ChildNodes[1].ChildNodes[0];

                    foreach (XmlNode node in ul.ChildNodes)
                    {
                        user = new CNUserInfo();

                        user.Avatar = node.ChildNodes[0].ChildNodes[0].ChildNodes[0].Attributes["src"].Value;
                        user.Name = node.ChildNodes[1].ChildNodes[0].InnerText;
                        user.BlogApp = node.ChildNodes[1].ChildNodes[0].Attributes["href"].Value.Split('/')[2];

                        list_followees.Add(user);
                    }

                    return list_followees;
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
        /// 获取当前登录用户的站内信（收件箱、发件箱）
        /// </summary>
        /// <param name="inbox"></param>
        /// <param name="page_index"></param>
        /// <returns></returns>
        public async static Task<List<CNMessage>> GetCurrentUserMessage(bool inbox,int page_index)
        {
            try
            {
                string url = inbox ? _url_inbox_messages : _url_outbox_messages;
                url = string.Format(url, page_index) + "?t=" + DateTime.Now.Millisecond;
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    html = html.Split(new string[] { "<tbody>" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "</tbody>" }, StringSplitOptions.None)[0];
                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + "<result>" + html.Replace("type=\"checkbox\">", "type=\"checkbox\"/>") + "</result>";

                    List<CNMessage> list_messages = new List<CNMessage>();
                    CNMessage msg;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    XmlNode result = doc.ChildNodes[1];
                    foreach (XmlNode node in result.ChildNodes)
                    {
                        msg = new CNMessage();
                        if (inbox)
                        {
                            msg.Inbox = inbox;
                            msg.FromOrTo = node.ChildNodes[0].InnerText.Trim();
                            msg.Title = node.ChildNodes[1].InnerText.Trim();
                            msg.ID = node.ChildNodes[1].ChildNodes[0].Attributes["href"].Value.Split('/')[3];
                            msg.AuthorID = node.ChildNodes[0].ChildNodes[0].Attributes["href"].Value.Split('/')[4];
                        }
                        else
                        {
                            msg.Inbox = inbox;
                            msg.FromOrTo = node.ChildNodes[1].InnerText.Trim();
                            msg.Title = node.ChildNodes[2].InnerText.Trim();
                            msg.ID = node.ChildNodes[2].ChildNodes[0].Attributes["href"].Value.Split('/')[3];
                            msg.AuthorID = node.ChildNodes[1].ChildNodes[0].Attributes["href"].Value.Split('/')[4];
                        }

                        list_messages.Add(msg);
                    }

                    return list_messages;
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
        /// 获取站内信具体内容
        /// </summary>
        /// <param name="msg_id"></param>
        /// <returns></returns>
        public async static Task<List<CNMessageItem>> GetMessageItems(string msg_id)
        {
            try
            {
                string url = string.Format(_url_messages_items, msg_id) + "?t=" + DateTime.Now.Millisecond;
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    html = html.Split(new string[] { "<ul class=\"bubble-wrap\">" }, StringSplitOptions.None)[1]
                        .Split(new string[] { "</ul>"}, StringSplitOptions.None)[0];

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + "<result>" + html.Replace("<br>","") + "</result>";

                    List<CNMessageItem> list_items = new List<CNMessageItem>();
                    CNMessageItem item;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    XmlNode result = doc.ChildNodes[1];

                    foreach (XmlNode node in result.ChildNodes)
                    {
                        item = new CNMessageItem();

                        item.AuthorAvatar = node.ChildNodes[0].ChildNodes[0].Attributes["src"].Value;
                        item.AuthorHome = node.ChildNodes[0].Attributes["href"].Value;
                        item.AuthorName = node.ChildNodes[0].Attributes["title"].Value;
                        item.MsgID = msg_id;
                        item.Send = node.ChildNodes[0].Attributes["class"] == null ? false : true; //发送者有样式
                        item.Time = node.ChildNodes[1].InnerText;
                        item.Content = node.ChildNodes[2].ChildNodes[0].InnerText;

                        list_items.Add(item);
                    }
                    return list_items;
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
        /// 获取当前登录用户的收藏
        /// </summary>
        /// <param name="page_index"></param>
        /// <returns></returns>
        public async static Task<List<CNCollection>> GetCurrentUserCollection(int page_index)
        {
            try
            {
                string url = string.Format(_url_collections, page_index) + "?t=" + DateTime.Now.Millisecond;
                string html = await BaseService.SendGetRequest(url);

                if (html != null)
                {
                    List<CNCollection> list_collections = new List<CNCollection>();
                    CNCollection collection;

                    html = html.Split(new string[] { "<div id=\"main\">" }, StringSplitOptions.None)[2]
                        .Split(new string[] { "<div id=\"right_sidebar\">" }, StringSplitOptions.None)[0];

                    html = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + "<div><div>" + html.Replace(")\">",")\"/>").Replace("&nbsp;","");

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html);

                    XmlNode result = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];
                    foreach (XmlNode node in result.ChildNodes)
                    {
                        collection = new CNCollection();
                        collection.Title = node.ChildNodes[0].ChildNodes[1].ChildNodes[0].InnerText.Trim();
                        if (node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes.Count == 2)
                        {
                            collection.Summary = node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                            collection.RawUrl = node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText;

                        }
                        else
                        {
                            collection.Summary = "";
                            collection.RawUrl= node.ChildNodes[0].ChildNodes[1].ChildNodes[1].ChildNodes[0].InnerText;
                        }
                        collection.CollectionTime = "收藏于 " + node.ChildNodes[0].ChildNodes[1].ChildNodes[2].ChildNodes[1].InnerText;
                        collection.CollectionCount= node.ChildNodes[0].ChildNodes[1].ChildNodes[2].ChildNodes[2].InnerText + "次";
                        collection.Category = "标签 " + node.ChildNodes[0].ChildNodes[1].ChildNodes[2].ChildNodes[4].ChildNodes[0].InnerText;

                        list_collections.Add(collection);
                    }
                    return list_collections;
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
        /// 推荐博客（只有推荐功能 没有反对功能）
        /// </summary>
        /// <param name="blog_app"></param>
        /// <param name="post_id"></param>
        /// <returns>返回推荐结果、文本消息</returns>
        public async static Task<object[]> DiggBlog(string blog_app, string post_id)
        {
            try
            {
                string url = _url_digg_blog;
                string body = "{{\"blogApp\":\"{0}\",\"postId\":{1},\"voteType\":\"Digg\",\"isAbandoned\":false}}";
                body = string.Format(body, blog_app, post_id);

                string json_result = await BaseService.SendPostRequest(url, body);

                if (json_result != null)
                {
                    bool r = json_result.Contains("true") ? true : false;
                    string msg = json_result.Split(new string[] { "\"Message\":" }, StringSplitOptions.None)[1]
                        .Split(',')[0];

                    return new object[] { r, msg };
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
        /// 推荐新闻（只有推荐功能 没有反对功能）
        /// </summary>
        /// <param name="content_id"></param>
        /// <returns>返回推荐结果、文本消息</returns>
        public async static Task<object[]> DiggNews(string content_id)
        {
            try
            {
                string url = _url_digg_news;
                string body = "{{\"contentId\":{0},\"action\":\"agree\"}}";
                body = string.Format(body, content_id);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    bool r = json_result.Contains("true") ? true : false;
                    string msg = json_result.Split(new string[] { "\"Message\":" }, StringSplitOptions.None)[1]
                        .Split('}')[0];

                    return new object[] { r, msg };
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
        /// 关注某人
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async static Task<bool> FocusSomeOne(string uid)
        {
            try
            {
                string url = _url_follow_user;
                string body = "{{userId:{0},remark:\"\"}}";
                body = string.Format(body, uid);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    if (json_result.Contains("true"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 发送私信
        /// </summary>
        /// <param name="to"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async static Task<object[]> SendMsg(string to, string title, string content)
        {
            try
            {
                string url = _url_sendmsg;
                string body = "{{\"incept\":\"{0}\",\"title\":\"{1}\",\"content\":\"{2}\"}}";
                body = string.Format(body, to, title, content);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    bool r = json_result.Contains("success") ? true : false;
                    string msg = json_result.Split(new string[] { "\"content\":" }, StringSplitOptions.None)[1]
                        .Split('}')[0];

                    return new object[] { r, msg };
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
        /// 回复私信
        /// </summary>
        /// <param name="msg_id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async static Task<object[]> ReplyMsg(string msg_id, string content)
        {
            try
            {
                string url = _url_replymsg;
                string body = "{{\"id\":\"{0}\",\"content\":\"{1}\"}}";
                body = string.Format(body, msg_id, content);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    bool r = json_result.Contains("success") ? true : false;
                    string msg = json_result.Split(new string[] { "\"content\":" }, StringSplitOptions.None)[1]
                        .Split('}')[0];

                    return new object[] { r, msg };
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
        /// 发布博客评论
        /// </summary>
        /// <param name="blog_app"></param>
        /// <param name="post_id"></param>
        /// <param name="parent_comment_id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async static Task<object[]> AddBlogComment(string blog_app, string post_id, string parent_comment_id, string comment)
        {
            try
            {
                string url = _url_add_blog_comment;
                string body = "{{\"blogApp\":\"{0}\",\"postId\":{1},\"body\":\"{2}\",\"parentCommentId\":\"{3}\"}}";
                body = string.Format(body, blog_app, post_id, comment, parent_comment_id);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    bool r = json_result.Contains("true") ? true : false;
                    string msg = json_result.Split(new string[] { "\"Message\":" }, StringSplitOptions.None)[1]
                        .Split(new string[] { ",\"Duration\"" }, StringSplitOptions.None)[0];

                    string duration = json_result.Split(new string[] { "\"Duration\":" }, StringSplitOptions.None)[1]
                        .Split('}')[0];

                    return new object[] { r, msg, duration };
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
        /// 发布新闻评论
        /// </summary>
        /// <param name="content_id"></param>
        /// <param name="content"></param>
        /// <param name="parent_comment_id"></param>
        /// <returns></returns>
        public async static Task<bool> AddNewsComment(string content_id, string content, string parent_comment_id)
        {
            try
            {
                string url = _url_add_news_comment;
                string body = "{{\"ContentID\":{0},\"Content\":\"{1}\",\"strComment\":\"\",\"parentCommentId\":\"{2}\",\"title\":\"\"}}";
                body = string.Format(body, content_id, content, parent_comment_id);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 获取当前登录用户的文摘标签
        /// </summary>
        /// <returns></returns>
        public async static Task<string[]> GetWZTags()
        {
            try
            {
                string url = _url_get_wztags;
                string html_result = await BaseService.SendPostRequest(url, "{}");
                if (html_result != null)
                {
                    html_result = "<?xml version =\"1.0\" encoding=\"utf - 8\" ?> " + html_result;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(html_result);

                    return doc.ChildNodes[1].InnerText.Split(',');
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
        /// 增加文摘
        /// </summary>
        /// <param name="wz_url"></param>
        /// <param name="wz_title"></param>
        /// <param name="wz_tags"></param>
        /// <param name="wz_summary"></param>
        /// <returns></returns>
        public async static Task<object[]> AddWZ(string wz_url, string wz_title, string wz_tags, string wz_summary)
        {
            try
            {
                string url = _url_add_wz;
                string body = "{{\"wzLinkId\":0,\"url\":\"{0}\",\"title\":\"{1}\",\"tags\":\"{2}\",\"summary\":\"{3}\",\"isPublic\":1,\"linkType\":1}}";
                body = string.Format(body, wz_url, wz_title, wz_tags, wz_summary);

                string json_result = await BaseService.SendPostRequest(url, body);
                if (json_result != null)
                {
                    bool r = json_result.Contains("true") ? true: false;
                    string msg = json_result.Split(new string[] { "\"message\":" }, StringSplitOptions.None)[1]
                        .Split('}')[0];

                    return new object[] { r, msg };
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
