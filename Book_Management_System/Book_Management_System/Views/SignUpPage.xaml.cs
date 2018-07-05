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
using Book_Management_System.Models;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

/// <summary>
/// 本页面主要用于注册
/// 注册需要验证输入信息的合法性
/// 最后添加读者到数据库
/// 注册完成后提示注册成功，然后进入用户个人界面UserPage
/// 
/// todo:
/// 完成新用户的添加、同步进入数据库
/// </summary>

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SignUpPage : Page
    {
        DataTransferManager dataTransferManager;


        public SignUpPage()
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

            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/mainpage.png", UriKind.Absolute))
            };
            gd_backimage.Background = imageBrush;
        }

        ViewModels.ManagementViewModels ViewModel { get; set; }

        async void OnShareDateRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/wallhaven-588148.png"));
            dp.Properties.Title = ViewModel.SelectedItem.name;
            dp.Properties.Description = ViewModel.SelectedItem.description;
            dp.SetStorageItems(new List<StorageFile> { photoFile });
            deferral.Complete();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

        User user = new User("", "", "", "");

        public void signupButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = "";
            int res = -1;
            if (UserName.Text == "")
            {
                var i = new MessageDialog("Please enter your username!").ShowAsync();
                temp += "a";
                return;
            }
            if (Password.Password == "")
            {
                var i = new MessageDialog("Please enter your password!").ShowAsync();
                temp += "a";
                return;
            }
            if (Password.Password.Length < 8)
            {
                var i = new MessageDialog("Your password must longer than 8 characters!").ShowAsync();
                temp += "a";
                return;
            }
            if (Phone.Text.Length != 11)
            {
                temp += "a";
                var i = new MessageDialog("Your phone must be 11 numbers!").ShowAsync();
                return;
            }
            if (Email.Text == "")
            {
                var i = new MessageDialog("Please enter your email!").ShowAsync();
                temp += "a";
                return;
            }

            if (temp == "")
            {
                res = ViewModel.AddUser(UserName.Text, Password.Password, Phone.Text, Email.Text);
            }
            
            if (res == 1)
            {
                Frame.Navigate(typeof(UserPage), UserName.Text);
            }
        }

        public void returnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (user.valid_passwardAsync(Password.Password))
            {
                password_status.Text = "valid";
            }
            else
            {
                password_status.Text = "invalid";
            }
        }

        private void Phone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (user.valid_phoneAsync(Phone.Text.ToString()))
            {
                phone_status.Text = "valid";
            }
            else
            {
                phone_status.Text = "invalid";
            }
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (user.Valid_usernameAsync(UserName.Text.ToString()))
            {
                username_status.Text = "valid";
            }
            else
            {
                username_status.Text = "invalid";
            }
        }

        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (user.valid_emailAsync(Email.Text.ToString()))
            {
                email_status.Text = "valid";
            }
            else
            {
                email_status.Text = "invalid";
            }
        }
    }
}
