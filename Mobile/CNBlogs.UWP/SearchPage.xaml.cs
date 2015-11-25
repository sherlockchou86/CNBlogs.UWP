using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class SearchPage : Page
    {
        /// <summary>
        /// 当前显示的博客列表
        /// </summary>
        private ObservableCollection<CNBlog> _list_blogs = new ObservableCollection<CNBlog>();
        /// <summary>
        /// 当前显示的博主列表
        /// </summary>
        private ObservableCollection<CNBloger> _list_blogers = new ObservableCollection<CNBloger>();
        public SearchPage()
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
            if (e.NavigationMode == NavigationMode.Back)
            {
                //恢复状态
                if (App.PageState != null && App.PageState["SearchPage_PivotSelectIndex"] != null)
                {
                    pivot.SelectedIndex = (int)App.PageState["SearchPage_PivotSelectIndex"];
                    SearchBox.Text = (string)App.PageState["SearchPage_SearchKeys"];
                    SearchBox_QuerySubmitted(null, null);

                }
            }
            else
            {
                object[] parameters = e.Parameter as object[];
                if (parameters != null && parameters.Length == 2)
                {
                    SearchBox.Text = parameters[0].ToString();  //关键字
                    pivot.SelectedIndex = (int)parameters[1];  //0找博客  1找博主
                    SearchBox_QuerySubmitted(null, null);
                }
                else
                {
                    Loading.IsActive = false;
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
            if (App.PageState == null)
            {
                App.PageState = new Dictionary<string, object>();
            }
            App.PageState.Remove("SearchPage_PivotSelectIndex");
            App.PageState.Remove("SearchPage_SearchKeys");
            App.PageState.Add("SearchPage_SearchKeys", SearchBox.Text);
            App.PageState.Add("SearchPage_PivotSelectIndex", pivot.SelectedIndex);
        }
        /// <summary>
        /// 切换搜索方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as PivotItem).Header.Equals("找博客"))
            {
                SearchBox.PlaceholderText = "找博客关键字";
                ListCount.Text = _list_blogs.Count.ToString();
            }
            else
            {
                SearchBox.PlaceholderText = "找博主关键字";
                ListCount.Text = _list_blogers.Count.ToString();
            }
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

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
        /// 点击博主 查看主页博客
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlogerListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (e.ClickedItem as CNBloger).BlogApp, (e.ClickedItem as CNBloger).BlogerName });
        }
        /// <summary>
        /// 标题栏搜索框发起查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Loading.IsActive = true;
            if (pivot.SelectedIndex == 0)  //找博客
            {
                List<CNBlog> refresh_blogs = await SearchService.SearchBlogs(SearchBox.Text, 1);
                if(refresh_blogs!=null)
                {
                    _list_blogs.Clear();
                    refresh_blogs.ForEach((b) => _list_blogs.Add(b));
                    ListCount.Text = _list_blogs.Count.ToString();
                    Loading.IsActive = false;
                }
            }
            else //找博主
            {
                List<CNBloger> refresh_blogers = await SearchService.SearchBloger(SearchBox.Text);
                if(refresh_blogers!=null)
                {
                    _list_blogers.Clear();
                    refresh_blogers.ForEach((b) => _list_blogers.Add(b));
                    ListCount.Text = _list_blogers.Count.ToString();
                    Loading.IsActive = false;
                }
            }
        }
        /// <summary>
        /// 点击博主昵称  查看主页博客
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserHome), new object[] { (sender as HyperlinkButton).Tag, (sender as HyperlinkButton).Content });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await(new MessageDialog("先点击查看全文再推荐哟!")).ShowAsync();
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
