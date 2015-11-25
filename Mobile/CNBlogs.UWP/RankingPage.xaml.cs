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
    public sealed partial class RankingPage : Page
    {
        /// <summary>
        /// 48小时阅读排行
        /// </summary>
        private CN48TopViewList _list_48Views;
        /// <summary>
        /// 10天推荐排行
        /// </summary>
        private CN10TopDiggList _list_10Diggs;
        /// <summary>
        /// 新闻推荐排行
        /// </summary>
        private CNTopNewsList _list_newsDiggs;
        /// <summary>
        /// 推荐博主
        /// </summary>
        private CNTopBlogerList _list_blogerDiggs;
        public RankingPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }

            _list_48Views = new CN48TopViewList();
            _list_10Diggs = new CN10TopDiggList();
            _list_newsDiggs = new CNTopNewsList();
            _list_blogerDiggs = new CNTopBlogerList();

            _list_10Diggs.DataLoaded += _list_10Diggs_DataLoaded;
            _list_10Diggs.DataLoading += _list_10Diggs_DataLoading;

            _list_48Views.DataLoading += _list_48Views_DataLoading;
            _list_48Views.DataLoaded += _list_48Views_DataLoaded;

            _list_newsDiggs.DataLoaded += _list_newsDiggs_DataLoaded;
            _list_newsDiggs.DataLoading += _list_newsDiggs_DataLoading;

            _list_blogerDiggs.DataLoaded += _list_blogerDiggs_DataLoaded;
            _list_blogerDiggs.DataLoading += _list_blogerDiggs_DataLoading;
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                //恢复状态
                if (App.PageState != null && App.PageState["RankingPage_PivotSelectIndex"]!=null)
                {
                    pivotRanks.SelectedIndex = (int)App.PageState["RankingPage_PivotSelectIndex"];
                }
            }
        }
        /// <summary>
        /// 页面离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //保存状态
            if (App.PageState == null)
            {
                App.PageState = new Dictionary<string, object>();
            }
            App.PageState.Remove("RankingPage_PivotSelectIndex");
            App.PageState.Add("RankingPage_PivotSelectIndex", pivotRanks.SelectedIndex); //保存当前pivot的选择项
        }
        /// <summary>
        /// 点击查看48小时阅读排行榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View48ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogContentPage),new object[] {e.ClickedItem });
        }
        /// <summary>
        /// 点击查看十天推荐排行榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Digg10ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BlogContentPage),new object[] { e.ClickedItem});
        }
        /// <summary>
        /// 点击查看新闻推荐排行榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsContentPage),new object[] { e.ClickedItem});
        }
        /// <summary>
        /// 点击推荐博主 查看主页博客
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlogerGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome),new object[] {(e.ClickedItem as CNBloger).BlogApp, (e.ClickedItem as CNBloger).BlogerName });
        }
        /// <summary>
        /// 点击博主昵称  查看主页博客
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag.ToString(),(sender as HyperlinkButton).Content });
        }
        /// <summary>
        /// 选项卡（pivot)选择变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;
            if (p.SelectedIndex == 0)  //48小时阅读排行
            {
                ListCount.Text = _list_48Views.TotalCount.ToString();
            }
            else if (p.SelectedIndex == 1)  //10天推荐排行
            {
                ListCount.Text =_list_10Diggs.TotalCount.ToString();
            }
            else if (p.SelectedIndex == 2)  //新闻推荐排行
            {
                ListCount.Text = _list_newsDiggs.TotalCount.ToString();
            }
            else //推荐博主
            {
                ListCount.Text = _list_blogerDiggs.TotalCount.ToString();
            }
        }



        private void _list_newsDiggs_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_newsDiggs_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_newsDiggs.TotalCount.ToString();
        }

        private void _list_48Views_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_48Views.TotalCount.ToString();
        }

        private void _list_48Views_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_10Diggs_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_10Diggs_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_10Diggs.TotalCount.ToString();
        }
        private void _list_blogerDiggs_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_blogerDiggs_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_blogerDiggs.TotalCount.ToString();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (pivotRanks.SelectedIndex == 0)  //48小时阅读
            {
                _list_48Views.DoRefresh();
                await View48ListView.LoadMoreItemsAsync();
            }
            else if (pivotRanks.SelectedIndex == 1)  //10天推荐
            {
                _list_10Diggs.DoRefresh();
                await Digg10ListView.LoadMoreItemsAsync();
            }
            else if (pivotRanks.SelectedIndex == 2)  //推荐新闻
            {
                _list_newsDiggs.DoRefresh();
                await NewsListView.LoadMoreItemsAsync();
            }
            else //推荐博主
            {
                _list_blogerDiggs.DoRefresh();
                await BlogerGridView.LoadMoreItemsAsync();
            }
        
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
