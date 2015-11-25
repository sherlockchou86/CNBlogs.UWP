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
    public sealed partial class NewsCommentPage : Page
    {
        private string _totalHtml = "";
        private CNNews _news;
        private string _at_comment_id = "";

        public NewsCommentPage()
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
                _news = parameters[0] as CNNews;

                NewsTitle.Text = _news.Title;
                NewsInfo.Text = _news.SourceName + " " + _news.PublishTime;

                _totalHtml = ChatBoxTool.BaseChatHtml;
                if (App.Theme == ApplicationTheme.Dark)
                {
                    _totalHtml += "<style>body{background-color:black;color:white;}</style>";
                }
                NewsComment.NavigateToString(_totalHtml);

                List<CNNewsComment> refresh_comments = await NewsService.GetNewsCommentsAysnc(_news.ID, 1, 200);

                if (refresh_comments != null)
                {
                    string comments = "";
                    foreach (CNNewsComment comment in refresh_comments)
                    {
                        if ((App.LoginedUser != null) && (App.LoginedUser.Name == comment.AuthorName))
                        {
                            comments += ChatBoxTool.Send(comment.AuthorAvatar,
                                comment.AuthorName, comment.Content, comment.PublishTime);
                        }
                        else
                        {
                            comments += ChatBoxTool.Receive(comment.AuthorAvatar,
                            comment.AuthorName,
                            comment.Content, comment.PublishTime, comment.ID);
                        }
                    }
                    comments += "<a id='ok'></a>";

                    _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + comments + "<a id='ok'></a>";

                    NewsComment.NavigateToString(_totalHtml);
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
            if(this.Frame.CanGoBack)
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
                bool result = await UserService.AddNewsComment(_news.ID, comment, _at_comment_id);
                if (result)
                {
                    _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + ChatBoxTool.Send(App.LoginedUser.Avatar, App.LoginedUser.Name, comment, DateTime.Now.ToString()) + "<a id='ok'></a>";
                    NewsComment.NavigateToString(_totalHtml);
                }
                else
                {
                    await(new MessageDialog("评论失败!")).ShowAsync();
                }
                MyComment.IsEnabled = true;
                MyComment.Text = "";
            }
        }
        /// <summary>
        /// 点击昵称 头像 进行@操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewsComment_ScriptNotify(object sender, NotifyEventArgs e)
        {
            MyComment.Text += "@" + e.Value.Split('-')[0] + " ";
            _at_comment_id = e.Value.Split('-')[1];  //@评论的id
        }
        /// <summary>
        /// 点击刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            _totalHtml = ChatBoxTool.BaseChatHtml;
            if (App.Theme == ApplicationTheme.Dark)
            {
                _totalHtml += "<style>body{background-color:black;color:white;}</style>";
            }
            NewsComment.NavigateToString(_totalHtml);

            List<CNNewsComment> refresh_comments = await NewsService.GetNewsCommentsAysnc(_news.ID, 1, 200);

            if (refresh_comments != null)
            {
                string comments = "";
                foreach (CNNewsComment comment in refresh_comments)
                {
                    if ((App.LoginedUser != null) && (App.LoginedUser.Name == comment.AuthorName))
                    {
                        comments += ChatBoxTool.Send(comment.AuthorAvatar,
                            comment.AuthorName, comment.Content, comment.PublishTime);
                    }
                    else
                    {
                        comments += ChatBoxTool.Receive(comment.AuthorAvatar,
                        comment.AuthorName,
                        comment.Content, comment.PublishTime, comment.ID);
                    }
                }
                comments += "<a id='ok'></a>";

                _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + comments + "<a id='ok'></a>";

                NewsComment.NavigateToString(_totalHtml);
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
