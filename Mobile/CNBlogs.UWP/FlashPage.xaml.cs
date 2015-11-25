using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class FlashPage : Page
    {
        public FlashPage()
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
        }
        /// <summary>
        /// 点击闪存 查看详细内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlashListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlashContent.Visibility == Visibility.Visible) //子页面显示
            {
                this.FlashContent.Navigate(typeof(FlashCommentPage), new object[] { null, false });
            }
            else  //当前页面显示
            {
                this.Frame.Navigate(typeof(FlashCommentPage), new object[] { null, true });
            }
        }
    }
}
