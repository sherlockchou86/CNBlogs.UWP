using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class SearchDialog : ContentDialog
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string KeyWords
        {
            get;
            private set;
        }
        /// <summary>
        /// 搜索类型 0博客 1博主
        /// </summary>
        public int SearchType
        {
            get;
            private set;
        }
        public SearchDialog()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 点击确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (SearchKey.Text.Equals(""))
            {
                SearchKey.Header = "请输入搜索关键字!";
                args.Cancel = true;
            }
            else
            {
                KeyWords = SearchKey.Text;
                SearchType = ((bool)BlogRadioButton.IsChecked) ? 0 : 1;
            }
        }
        /// <summary>
        /// 点击取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
