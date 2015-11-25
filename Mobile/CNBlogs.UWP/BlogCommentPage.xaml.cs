using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Models;
using CNBlogs.UWP.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace CNBlogs.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlogCommentPage : Page
    {
        string _totalHtml = "";
        CNBlog _blog;
        string _at_comment_id = "";

        public BlogCommentPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            
        }
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            object[] parameters = e.Parameter as object[];
            if (parameters != null && parameters.Length == 1)
            {
                _blog = parameters[0] as CNBlog;
                BlogTitle.Text = _blog.Title;
                Author.Content = _blog.AuthorName;
                PubishTime.Text = _blog.PublishTime;

                _totalHtml = ChatBoxTool.BaseChatHtml;
                if (App.Theme == ApplicationTheme.Dark)
                {
                    _totalHtml += "<style>body{background-color:black;color:white;}</style>";
                }
                BlogComment.NavigateToString(_totalHtml);
                List<CNBlogComment> list_comments = await BlogService.GetBlogCommentsAsync(_blog.ID, 1, 199);

                if(list_comments != null)
                {
                    string comments = "";
                    foreach(CNBlogComment comment in list_comments)
                    {
                        if ((App.LoginedUser != null)&&(comment.AuthorName == App.LoginedUser.Name))
                        {
                            comments += ChatBoxTool.Send(comment.AuthorAvatar,
                                comment.AuthorName == _blog.AuthorName ? "[博主]" + _blog.AuthorName : comment.AuthorName,
                                comment.Content, comment.PublishTime);
                        }
                        else
                        {
                            comments += ChatBoxTool.Receive(comment.AuthorAvatar,
                                comment.AuthorName == _blog.AuthorName ? "[博主]" + _blog.AuthorName : comment.AuthorName,
                                comment.Content, comment.PublishTime, comment.ID);
                        }
                    }
                    //comments += "<a id='ok'></a>";

                    _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + comments + "<a id='ok'></a>";
                    BlogComment.NavigateToString(_totalHtml);
                    Loading.IsActive = false;
                }
            }
        }
        /// <summary>
        /// 点击后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void MyComment_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (App.LoginedUser == null)
            {
                await (new MessageDialog("请先登录!")).ShowAsync();
                return;
            }
            if (!MyComment.Text.Equals(""))
            {
                MyComment.IsEnabled = false;
                string comment = MyComment.Text;
                MyComment.Text = "正在发送评论...";
                object[] result = await UserService.AddBlogComment(_blog.BlogApp, _blog.ID, _at_comment_id, comment);
                if (result != null)
                {
                    _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + ChatBoxTool.Send(App.LoginedUser.Avatar, App.LoginedUser.Name, comment, DateTime.Now.ToString()) + "<a id='ok'></a>";
                    BlogComment.NavigateToString(_totalHtml);
                }
                else
                {
                    await (new MessageDialog("评论失败!")).ShowAsync();
                }
                MyComment.IsEnabled = true;
                MyComment.Text = "";         
            }
        }
        /// <summary>
        /// 点击昵称 进行@操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlogComment_ScriptNotify(object sender, NotifyEventArgs e)
        {
            MyComment.Text += "@"+ e.Value.Split('-')[0] + " ";
            _at_comment_id = e.Value.Split('-')[1];  //@评论的id
        }
        /// <summary>
        /// 点击博主昵称 转到博客主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Author_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { _blog.BlogApp, _blog.AuthorName });
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _totalHtml = ChatBoxTool.BaseChatHtml;
            if (App.Theme == ApplicationTheme.Dark)
            {
                _totalHtml += "<style>body{background-color:black;color:white;}</style>";
            }
            BlogComment.NavigateToString(_totalHtml);
            Loading.IsActive = true;
            List<CNBlogComment> list_comments = await BlogService.GetBlogCommentsAsync(_blog.ID, 1, 199);

            if (list_comments != null)
            {
                string comments = "";
                foreach (CNBlogComment comment in list_comments)
                {
                    if ((App.LoginedUser != null) && (comment.AuthorName == App.LoginedUser.Name))
                    {
                        comments += ChatBoxTool.Send(comment.AuthorAvatar, 
                            comment.AuthorName == _blog.AuthorName ? "[博主]" + _blog.AuthorName : comment.AuthorName,
                            comment.Content, comment.PublishTime);
                    }
                    else
                    {
                        comments += ChatBoxTool.Receive(comment.AuthorAvatar,
                            comment.AuthorName == _blog.AuthorName ? "[博主]" + _blog.AuthorName : comment.AuthorName,
                            comment.Content, comment.PublishTime, comment.ID);
                    }
                }
                //comments += "<a id='ok'></a>";

                _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + comments + "<a id='ok'></a>";

                BlogComment.NavigateToString(_totalHtml);
                Loading.IsActive = false;
            }
        }
        /// <summary>
        /// 打开主菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).ShowNavigationBarOneTime();
        }
    }
}
