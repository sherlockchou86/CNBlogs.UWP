using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.HTTP;
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

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace CNBlogs.UWP
{
    public sealed partial class SendMsgDialog : ContentDialog
    {
        public SendMsgDialog(string to = null)
        {
            this.InitializeComponent();
            if (to != null)
            {
                To.Text = to;
                To.IsReadOnly = true;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (To.Text.Equals(""))
            {
                To.Header = "请输入收信人";
                args.Cancel = true;
            }
            else if (Title.Text.Equals(""))
            {
                Title.Header = "请输入标题";
                args.Cancel = true;
            }
            else if (Content.Text.Equals(""))
            {
                Content.Header = "请输入内容";
                args.Cancel = true;
            }
            else
            {
                Sending.Visibility = Visibility.Visible;
                args.Cancel = true;
                IsPrimaryButtonEnabled = false;
                object[] result = await UserService.SendMsg(To.Text, Title.Text, Content.Text);
                if (result != null)
                {
                    if ((bool)result[0])
                    {
                        //成功  自动关闭
                        Hide();
                    }
                    else
                    {
                        Tip.Text = result[1].ToString();
                        IsPrimaryButtonEnabled = true;
                        Sending.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    Tip.Text = "发送失败!";
                    IsPrimaryButtonEnabled = true;
                    Sending.Visibility = Visibility.Collapsed;
                }

            }
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
