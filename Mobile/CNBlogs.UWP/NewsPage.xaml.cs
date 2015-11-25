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
    public sealed partial class NewsPage : Page
    {
        /// <summary>
        /// 当前页面加载的新闻列表
        /// </summary>
        private CNNewsList _list_news;
        public NewsPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            _list_news = new CNNewsList();
            _list_news.DataLoaded += _list_news_DataLoaded;
            _list_news.DataLoading += _list_news_DataLoading;
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //没有加载参数
        }
        /// <summary>
        /// 点击查看新闻内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsContentPage), new object[] { e.ClickedItem });
        }

        /// <summary>
        /// 新闻开始加载
        /// </summary>
        private void _list_news_DataLoading()
        {
            Loading.IsActive = true;
        }
        /// <summary>
        /// 新闻加载完毕
        /// </summary>
        private void _list_news_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_news.TotalCount.ToString();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _list_news.DoRefresh();
            await NewsListView.LoadMoreItemsAsync();
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
