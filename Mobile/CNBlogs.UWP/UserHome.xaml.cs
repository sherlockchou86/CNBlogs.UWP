using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.Data;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Models;
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
    public sealed partial class UserHome : Page
    {
        /// <summary>
        /// 当前博主的blog_app
        /// </summary>
        private string _blog_app;
        /// <summary>
        /// 当前页面加载的博客列表
        /// </summary>
        private CNUserBlogList _list_blogs;
        public UserHome()
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null)
            {
                if(parameters.Length == 2)  //blogapp  nickname
                {
                    _blog_app = parameters[0].ToString();
                    PageTitle.Text = parameters[1].ToString() + " 的博客";

                    BlogsListView.ItemsSource = _list_blogs = new CNUserBlogList(_blog_app);

                    _list_blogs.DataLoaded += _list_blogs_DataLoaded;
                    _list_blogs.DataLoading += _list_blogs_DataLoading;
                }
            }
        }

        /// <summary>
        /// 点击查看博客正文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlogsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogContentPage), new object[] { e.ClickedItem });
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
        /// 查看博主主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag.ToString(), (sender as HyperlinkButton).Content });
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _list_blogs.DoRefresh();
            await BlogsListView.LoadMoreItemsAsync();
        }
        /// <summary>
        /// 详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MoreInfo_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoginedUser == null)
            {
                await (new MessageDialog("请先登录!")).ShowAsync();
                return;
            }
            this.Frame.Navigate(typeof(UserInfoPage), new object[] { _blog_app });
        }

        private void _list_blogs_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_blogs_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_blogs.TotalCount.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await (new MessageDialog("先点击查看全文再推荐哟!")).ShowAsync();
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
