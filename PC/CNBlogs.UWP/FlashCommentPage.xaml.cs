using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.Tools;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class FlashCommentPage : Page
    {
        private string _totalHtml = "";
        public FlashCommentPage()
        {
            this.InitializeComponent();
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
                if (parameters.Length == 2 && !(bool)parameters[1])
                {
                    Back.Visibility = Visibility.Collapsed;
                }
                _totalHtml = ChatBoxTool.BaseChatHtml;

                //默认加载两条记录
                string receive = ChatBoxTool.Receive("http://pic.cnblogs.com/avatar/104032/20150821151916.png", "周建芝", "hello world,it's time to leave", "10:02");
                string send = ChatBoxTool.Send("http://pic.cnblogs.com/avatar/624159/20150505133758.png", "青柠檬", "以德服人", "11:32");

                _totalHtml = _totalHtml + receive + send + "<a id='ok'></a>";


                FlashComment.NavigateToString(_totalHtml);
            }
        }
        /// <summary>
        /// 发表回应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MyComment_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!MyComment.Text.Equals(""))
            {
                _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + ChatBoxTool.Send("http://pic.cnblogs.com/avatar/624159/20150505133758.png", "青柠檬", MyComment.Text, DateTime.Now.ToString()) + "<a id='ok'></a>";
                FlashComment.NavigateToString(_totalHtml);
            }
        }
        /// <summary>
        /// 点击昵称 头像 设置@操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlashComment_ScriptNotify(object sender, NotifyEventArgs e)
        {
            MyComment.Text += "@" + e.Value + " ";
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
    }
}
