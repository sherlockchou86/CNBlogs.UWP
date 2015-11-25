using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.HTTP;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AddWZDialog : ContentDialog
    {
        
        public AddWZDialog(string url,string title)
        {
            this.InitializeComponent();
            Loaded += AddWZDialog_Loaded;
            WZUrl.Text = url;
            WZTitle.Text = title;
        }
        /// <summary>
        /// 加载时 加载标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddWZDialog_Loaded(object sender, RoutedEventArgs e)
        {
            string[] tags = await UserService.GetWZTags();
            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    CheckBox ch = new CheckBox();
                    ch.Content = tag;
                    ch.Padding = new Thickness(0);
                    ch.FontSize = 13;
                    ch.VerticalContentAlignment = VerticalAlignment.Center;
                    ch.Click += Ch_Click;
                    Tags.Children.Add(ch);
                }
            }
        }
        /// <summary>
        /// 点击标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ch_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            if ((bool)ch.IsChecked)
            {
                if (WZTags.Text.Contains(ch.Content.ToString()))
                    return;
                WZTags.Text += (WZTags.Text.Equals("") ? "" : ",") + (sender as CheckBox).Content;
            }
            else
            {
                if (WZTags.Text.Contains(ch.Content.ToString()))
                {
                    List<string> l = WZTags.Text.Split(',').ToList();
                    l.Remove(ch.Content.ToString());
                    WZTags.Text =  string.Join(",", l.ToArray());
                }
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            UpLoading.Visibility = Visibility.Visible;
            IsPrimaryButtonEnabled = false;
            object[] result = await UserService.AddWZ(WZUrl.Text, WZTitle.Text, WZTags.Text, WZSummary.Text);
            if (result != null)
            {
                if ((bool)result[0])
                {
                    Hide();
                }
                else
                {
                    Tips.Text = result[1].ToString();
                    UpLoading.Visibility = Visibility.Collapsed;
                    IsPrimaryButtonEnabled = true;
                }
            }
            else
            {
                Tips.Text = "操作失败！";
                UpLoading.Visibility = Visibility.Collapsed;
                IsPrimaryButtonEnabled = true;
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
