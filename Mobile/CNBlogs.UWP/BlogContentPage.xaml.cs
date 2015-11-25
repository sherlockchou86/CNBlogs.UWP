using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CNBlogs.UWP.Models;
using CNBlogs.UWP.HTTP;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace CNBlogs.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlogContentPage : Page
    {
        /// <summary>
        /// 当前查看博客
        /// </summary>
        private CNBlog _blog;
        public BlogContentPage()
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
            if (parameters != null)
            {
                if(parameters.Length==1 && (parameters[0] as CNBlog)!=null)
                {
                    _blog = parameters[0] as CNBlog;

                    BlogTitle.Text = _blog.Title;
                    AuthorName.Content = _blog.AuthorName;
                    PublishTime.Text = _blog.PublishTime;
                    Views.Text = _blog.Views;
                    Diggs.Text = "["+_blog.Diggs + "]";
                    Comments.Text = _blog.Comments;
                    BitmapImage bi = new BitmapImage { UriSource = new Uri(_blog.AuthorAvator) };
                    Avatar.Source = bi;
                    AuthorName.Tag = _blog.BlogApp;
                    string blog_body = await BlogService.GetBlogContentAsync(_blog.ID);
                    if (blog_body != null)
                    {
                        if (App.Theme == ApplicationTheme.Dark)  //暗主题
                        {
                            blog_body += "<style>body{background-color:black;color:white;}</style>";
                        }
                        BlogContent.NavigateToString(blog_body);
                    }
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
        /// 点击标题栏上的查看评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogCommentPage), new object[] { _blog });
        }
        /// <summary>
        /// 点击标题栏上的刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            string blog_body = await BlogService.GetBlogContentAsync(_blog.ID);
            if (blog_body != null)
            {
                BlogContent.NavigateToString(blog_body);
                Loading.IsActive = false;
            }
        }
        /// <summary>
        /// 点击标题栏上的 推荐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Digg_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoginedUser == null)
            {
                await (new MessageDialog("请先登录!")).ShowAsync();
                return;
            }
            object[] result = await UserService.DiggBlog(_blog.BlogApp, _blog.ID);
            if (result != null)
            {
                if ((bool)result[0]) //推荐成功
                {
                    await (new MessageDialog((string)result[1])).ShowAsync();
                    Diggs.Text = "[" + (int.Parse(_blog.Diggs) + 1) + "]";
                }
                else
                {
                    await (new MessageDialog((string)result[1])).ShowAsync();
                }
            }
            else
            {
                await (new MessageDialog("操作失败!")).ShowAsync();
            }
        }
        /// <summary>
        /// 点击作者  跳转到作者主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthorName_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag.ToString(),(sender as HyperlinkButton).Content });
        }

        /// <summary>
        /// 点击推荐、评论小图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as SymbolIcon).Symbol == Symbol.Like)  //推荐
            {
                Digg_Click(null, null);
            }
            if((sender as SymbolIcon).Symbol == Symbol.Comment) //评论
            {
                this.Frame.Navigate(typeof(BlogCommentPage), new object[] { _blog });
            }
        }
        /// <summary>
        /// 收藏博客
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Collect_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoginedUser == null)
            {
                await (new MessageDialog("请先登录!")).ShowAsync();
            }
            else
            {
                await (new AddWZDialog(_blog.BlogRawUrl,_blog.Title)).ShowAsync();
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
