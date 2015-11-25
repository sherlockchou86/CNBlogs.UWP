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
    public sealed partial class SettingDialog : ContentDialog
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        MainPage _mainPage;
        public SettingDialog(MainPage mainPage)
        {
            this.InitializeComponent();
            _mainPage = mainPage;
            LoadConfig();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
        /// <summary>
        /// 加载数目选择变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex == -1)
                return;
            if ((sender as ComboBox).Name.Equals("NewsCount"))
            {
                localSettings.Values["NewsCountOneTime"] = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
                App.NewsCountOneTime = int.Parse(localSettings.Values["NewsCountOneTime"].ToString());
            }
            else
            {
                localSettings.Values["BlogCountOneTime"] = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
                App.BlogCountOneTime = int.Parse(localSettings.Values["BlogCountOneTime"].ToString());
            }
        }
        /// <summary>
        /// 清空登录信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["LoginInfos"] = null;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadConfig()
        {
            AlwayShowNavigation.IsOn = App.AlwaysShowNavigation;
            ThemeDark.IsOn = App.Theme == ApplicationTheme.Dark ? true : false;
            for (int i = 0; i < NewsCount.Items.Count; i++)
            {
                if ((NewsCount.Items[i] as ComboBoxItem).Content.ToString() == App.NewsCountOneTime.ToString())
                {
                    NewsCount.SelectedIndex = i;
                    break;
                }
            }
            for (int i = 0; i < BlogCount.Items.Count; i++)
            {
                if ((BlogCount.Items[i] as ComboBoxItem).Content.ToString() == App.BlogCountOneTime.ToString())
                {
                    BlogCount.SelectedIndex = i;
                    break;
                }
            }
        }
        /// <summary>
        /// 开关按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).Name.Equals("AlwayShowNavigation"))
            {
                if (AlwayShowNavigation.IsOn != App.AlwaysShowNavigation)
                {
                    localSettings.Values["AlwaysShowNavigation"] = AlwayShowNavigation.IsOn;
                    App.AlwaysShowNavigation = AlwayShowNavigation.IsOn;
                    _mainPage.ShowNavigationBar(AlwayShowNavigation.IsOn); //立即生效
                }
            }
            else
            {
                localSettings.Values["Theme"] = ThemeDark.IsOn ? ApplicationTheme.Dark.ToString() : ApplicationTheme.Light.ToString();      
            }
        }
    }
}
