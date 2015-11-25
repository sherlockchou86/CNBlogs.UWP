using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class CollectionPage : Page
    {
        /// <summary>
        /// 收藏列表
        /// </summary>
        CNCollectionList _list_collections;
        public CollectionPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            _list_collections = new CNCollectionList();
            _list_collections.DataLoaded += _list_collections_DataLoaded;
            _list_collections.DataLoading += _list_collections_DataLoading;
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="e"></param>
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //没有加载参数
        }
        /// <summary>
        /// 点击收藏  打开浏览器查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(CollectionContent), new object[] { (e.ClickedItem as CNCollection).RawUrl });
        }

        /// <summary>
        /// 列表开始加载数据
        /// </summary>
        private void _list_collections_DataLoading()
        {
            Loading.IsActive = true;
        }
        /// <summary>
        /// 列表加载数据完毕
        /// </summary>
        private void _list_collections_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_collections.TotalCount.ToString();
        }
        /// <summary>
        /// 点击刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _list_collections.DoRefresh();
            await CollectionListView.LoadMoreItemsAsync();
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
