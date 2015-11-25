using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.UWP.Models;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CNBlogs.UWP
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        #region APP状态
        /// <summary>
        /// 当前登录用户  未登录时为null
        /// </summary>
        internal static CNUserInfo LoginedUser
        {
            get; set;
        }
        /// <summary>
        /// 是否总是显示导航栏
        /// </summary>
        internal static bool AlwaysShowNavigation
        {
            get; set;
        }
        /// <summary>
        /// 每次加载新闻条数
        /// </summary>
        internal static int NewsCountOneTime
        {
            get; set;
        }
        /// <summary>
        /// 每次加载博客条数
        /// </summary>
        internal static int BlogCountOneTime
        {
            get; set;
        }
        /// <summary>
        /// 主题
        /// </summary>
        internal static ApplicationTheme Theme
        {
            get; set;
        }
        internal static Dictionary<string,object> PageState
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            LoadConfig();
            Application.Current.RequestedTheme = App.Theme;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // 当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // 确保当前窗口处于活动状态
            Window.Current.Activate();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        /// <summary>
        /// 加载配置项
        /// </summary>
        private void LoadConfig()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values["AlwaysShowNavigation"] != null)
            {
                App.AlwaysShowNavigation = (bool)localSettings.Values["AlwaysShowNavigation"];
            }
            else
            {
                App.AlwaysShowNavigation = true;
            }

            if (localSettings.Values["NewsCountOneTime"] != null)
            {
                App.NewsCountOneTime = int.Parse(localSettings.Values["NewsCountOneTime"].ToString());
            }
            else
            {
                App.NewsCountOneTime = 20;
            }

            if (localSettings.Values["BlogCountOneTime"] != null)
            {
                App.BlogCountOneTime = int.Parse(localSettings.Values["BlogCountOneTime"].ToString());
            }
            else
            {
                App.BlogCountOneTime = 20;
            }

            if (localSettings.Values["Theme"] != null)
            {
                App.Theme = (ApplicationTheme)Enum.Parse(typeof(ApplicationTheme),localSettings.Values["Theme"].ToString());
            }
            else
            {
                App.Theme = ApplicationTheme.Light;
            }
        }
    }
}
