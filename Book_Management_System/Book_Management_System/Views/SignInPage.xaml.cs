using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

/// <summary>
/// 本页面用于登录
/// 用户输入个人信息，系统验证信息是否正确
/// 登录成功后直接进入用户个人界面UserPage
/// 
/// todo:
/// 登陆判定
/// </summary>

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        DataTransferManager dataTransferManager;

        public SignInPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.ManagementViewModels();
            NavigationCacheMode = NavigationCacheMode.Enabled;
           
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnShareDateRequested);
        }

        ViewModels.ManagementViewModels ViewModel { get; set; }

        async void OnShareDateRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/vallhaven-588148.png"));
            dp.Properties.Title = ViewModel.SelectedItem.name;
            dp.Properties.Description = ViewModel.SelectedItem.description;
            dp.SetStorageItems(new List<StorageFile> { photoFile });
            deferral.Complete();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UserName.Text = "";
            Password.Password = "";
            UserName.PlaceholderText = "Entern your username";
            Password.PlaceholderText = "Entern your password";
            if (e.Parameter.GetType() == typeof(ViewModels.ManagementViewModels))
            {
                this.ViewModel = (ViewModels.ManagementViewModels)(e.Parameter);
            }

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void signinButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text == "")
            {
                var i = new MessageDialog("Please enter your username").ShowAsync();
            }
            if (Password.Password == "")
            {
                var i = new MessageDialog("Please enter your password").ShowAsync();
            }

            int a = 0;

            a = ViewModel.login(UserName.Text, Password.Password);

            if (a == 2)
            {
                Frame.Navigate(typeof(AdminPage), ViewModel);
            }
            else if (a == 1)
            {
                ViewModel.User = UserName.Text;
                Frame.Navigate(typeof(UserPage), ViewModel);
            }
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), UserName.Text);
        }
    }
}
