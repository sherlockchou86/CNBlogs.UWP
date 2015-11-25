using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.HTTP;
using CNBlogs.UWP.Models;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class NewsContentPage : Page
    {
        /// <summary>
        /// 当前显示新闻
        /// </summary>
        private CNNews _news;

        public NewsContentPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            RegisterForShare();
        }
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
                DataRequestedEventArgs>(this.ShareLinkHandler);
        }

        private void ShareLinkHandler(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "分享新闻";
            request.Data.Properties.Description = "向好友分享这篇新闻";
            request.Data.SetWebLink(new Uri(_news.NewsRawUrl));
        }
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Object[] parameters = e.Parameter as Object[];
            if(parameters != null && parameters.Length == 1)
            {
                _news = parameters[0] as CNNews;

                BlogTitle.Text = _news.Title;
                NewsSource.Text = _news.SourceName;
                PublishTime.Text = _news.PublishTime;
                Diggs.Text = "[" + _news.Diggs + "]";
                Views.Text = _news.Views;
                Comments.Text = _news.Comments;

                string news_content = await NewsService.GetNewsContentAsync(_news.ID);

                if(news_content != null)
                {
                    if (App.Theme == ApplicationTheme.Dark)  //暗主题
                    {
                        news_content += "<style>body{background-color:black;color:white;}</style>";
                    }
                    NewsContent.NavigateToString(news_content);
                }
                Loading.IsActive = false;
            }
        }
        /// <summary>
        /// 点击标题栏刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Loading.IsActive = true;
            string news_content = await NewsService.GetNewsContentAsync(_news.ID);
            if (news_content != null)
            {
                if (App.Theme == ApplicationTheme.Dark)  //暗主题
                {
                    news_content += "<style>body{background-color:black;color:white;}</style>";
                }
                NewsContent.NavigateToString(news_content);
                Loading.IsActive = false;
            }
        }
        /// <summary>
        /// 点击标题栏查看评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsCommentPage), new object[] { _news });
        }
        /// <summary>
        /// 点击标题栏后退
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
        /// 点击标题栏上的推荐
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
            object[] result = await UserService.DiggNews(_news.ID);
            if (result != null)
            {
                if ((bool)result[0]) //推荐成功
                {
                    await (new MessageDialog((string)result[1])).ShowAsync();
                    Diggs.Text = "[" + (int.Parse(_news.Diggs) + 1) + "]";
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
        /// 点击推荐 评论 小图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as SymbolIcon).Symbol == Symbol.Like)  //推荐
            {
                Digg_Click(null, null);
            }
            else  //评论
            {
                this.Frame.Navigate(typeof(NewsCommentPage), new object[] { _news });
            }
        }
        /// <summary>
        /// 点击收藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Collect_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoginedUser == null)
            {
                await(new MessageDialog("请先登录!")).ShowAsync();
            }
            else
            {
                await(new AddWZDialog(_news.NewsRawUrl,_news.Title)).ShowAsync();
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
        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }
    }
}
