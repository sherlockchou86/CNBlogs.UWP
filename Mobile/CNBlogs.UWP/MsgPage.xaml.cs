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
    public sealed partial class MsgPage : Page
    {
        /// <summary>
        /// 收件箱
        /// </summary>
        private CNMessageList _list_inbox_messages;
        /// <summary>
        /// 发件箱
        /// </summary>
        private CNMessageList _list_outbox_messages;
        public MsgPage()
        {
            this.InitializeComponent();
            if (App.AlwaysShowNavigation)
            {
                Home.Visibility = Visibility.Collapsed;
            }
            _list_inbox_messages = new CNMessageList(true);
            _list_outbox_messages = new CNMessageList(false);

            _list_inbox_messages.DataLoaded += _list_inbox_messages_DataLoaded;
            _list_inbox_messages.DataLoading += _list_inbox_messages_DataLoading;

            _list_outbox_messages.DataLoading += _list_outbox_messages_DataLoading;
            _list_outbox_messages.DataLoaded += _list_outbox_messages_DataLoaded;
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
        /// 查看收件箱 私信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InBoxListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (MsgContent.Visibility == Visibility.Visible)  //子页面显示
                {
                    this.MsgContent.Navigate(typeof(MsgDetailPage), new object[] { (e.AddedItems[0] as CNMessage).ID, false });
                }
                else  //当前页面显示
                {
                    this.Frame.Navigate(typeof(MsgDetailPage), new object[] { (e.AddedItems[0] as CNMessage).ID, true });
                }
            }
        }
        /// <summary>
        /// 查看发件箱 私信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutBoxListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (MsgContent.Visibility == Visibility.Visible)  //子页面显示
                {
                    this.MsgContent.Navigate(typeof(MsgDetailPage), new object[] { (e.AddedItems[0] as CNMessage).ID, false });
                }
                else  //当前页面显示
                {
                    this.Frame.Navigate(typeof(MsgDetailPage), new object[] { (e.AddedItems[0] as CNMessage).ID, true });
                }
            }
        }
        /// <summary>
        /// 点击昵称 转到个人详细信息页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage), new object[] { (sender as HyperlinkButton).Tag });
        }


        private void _list_outbox_messages_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_outbox_messages.TotalCount.ToString();
        }

        private void _list_outbox_messages_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_inbox_messages_DataLoading()
        {
            Loading.IsActive = true;
        }

        private void _list_inbox_messages_DataLoaded()
        {
            Loading.IsActive = false;
            ListCount.Text = _list_inbox_messages.TotalCount.ToString();
        }
        /// <summary>
        /// 选项卡选择变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MsgPivot.SelectedIndex == 0)  //收件箱
            {
                ListCount.Text = _list_inbox_messages.TotalCount.ToString();
            }
            else  //发件箱
            {
                ListCount.Text = _list_outbox_messages.TotalCount.ToString();
            }
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (MsgPivot.SelectedIndex == 0)  //收件箱
            {
                _list_inbox_messages.DoRefresh();
                await InBoxListView.LoadMoreItemsAsync();
            }
            else  //发件箱
            {
                _list_outbox_messages.DoRefresh();
                await OutBoxListView.LoadMoreItemsAsync();
            }
        }
        /// <summary>
        /// 发私信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendMsg_Click(object sender, RoutedEventArgs e)
        {
            await (new SendMsgDialog()).ShowAsync();
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
