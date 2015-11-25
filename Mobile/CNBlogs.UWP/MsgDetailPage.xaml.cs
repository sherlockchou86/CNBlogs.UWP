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
    public sealed partial class MsgDetailPage : Page
    {
        private string _totalHtml = "";
        private string _msg_id;
        public MsgDetailPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 界面加载
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            object[] parameters = e.Parameter as object[];  //页面导航参数
            if (parameters != null)
            {
                if (parameters.Length == 2 && !(bool)parameters[1])  //消息ID  页面显示方式
                {
                    Back.Visibility = Visibility.Collapsed;
                }
                _msg_id = parameters[0].ToString();
                _totalHtml = ChatBoxTool.BaseChatHtml;
                if (App.Theme == ApplicationTheme.Dark)
                {
                    _totalHtml += "<style>body{background-color:black;color:white;}</style>";
                }
                List<CNMessageItem> refresh_items = await UserService.GetMessageItems(_msg_id);
                if (refresh_items != null)
                {
                    string msgs = "";
                    foreach (CNMessageItem item in refresh_items)
                    {
                        if (item.Send)  //自己发的
                        {
                            msgs += ChatBoxTool.Send(item.AuthorAvatar, item.AuthorName, item.Content, item.Time);
                        }
                        else //别人发的
                        {
                            msgs += ChatBoxTool.Receive(item.AuthorAvatar, item.AuthorName, item.Content, item.Time);
                        }
                    }
                    //msgs += "<a id='ok'></a>";
                    _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + msgs + "<a id='ok'></a>";

                    MsgContent.NavigateToString(_totalHtml);
                    Loading.IsActive = false;
                }
            }
        }
        /// <summary>
        /// 发布回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void MyReply_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!MyReply.Text.Equals(""))
            {
                MyReply.IsEnabled = false;
                string reply = MyReply.Text;
                MyReply.Text = "正在发送回复...";
                object result = await UserService.ReplyMsg(_msg_id, reply);
                if (result != null)
                {
                    _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + ChatBoxTool.Send(App.LoginedUser.Avatar, App.LoginedUser.Name, reply, DateTime.Now.ToString()) + "<a id='ok'></a>";
                    MsgContent.NavigateToString(_totalHtml);
                }
                else
                {
                    await (new MessageDialog("回复失败!")).ShowAsync();
                }
                MyReply.IsEnabled = true;
                MyReply.Text = "";
            }
        }
        /// <summary>
        /// 点击昵称 头像 设置@操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgContent_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //没有@功能
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
            List<CNMessageItem> refresh_items = await UserService.GetMessageItems(_msg_id);
            if (refresh_items != null)
            {
                string msgs = "";
                foreach (CNMessageItem item in refresh_items)
                {
                    if (item.Send)  //自己发的
                    {
                        msgs += ChatBoxTool.Send(item.AuthorAvatar, item.AuthorName, item.Content, item.Time);
                    }
                    else //别人发的
                    {
                        msgs += ChatBoxTool.Receive(item.AuthorAvatar, item.AuthorName, item.Content, item.Time);
                    }
                }
                //msgs += "<a id='ok'></a>";
                _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + msgs + "<a id='ok'></a>";

                MsgContent.NavigateToString(_totalHtml);
                Loading.IsActive = false;
            }
        }
    }
}
