using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Book_Management_System.ViewModels;
using Book_Management_System.Models;
using Windows.UI.Core;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

/// <summary>
/// 用户在该页面进行个人信息修改
/// </summary>

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        DataTransferManager dataTransferManager;

        User user = new User();

        public UserInfoPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;

            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnShareDateRequested);

            UserName.PlaceholderText = "Original username: " + user.username;
            Password.PlaceholderText = "Enter your new password";
            Phone.PlaceholderText = "Original phone: " + user.phone;
            Email.PlaceholderText = "Original email: " + user.email;
        }

        private ViewModels.ManagementViewModels ViewModel;

        async void OnShareDateRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/background.jpg"));
            dp.Properties.Title = ViewModel.SelectedItem.title;
            dp.Properties.Description = ViewModel.SelectedItem.description;
            dp.SetStorageItems(new List<StorageFile> { photoFile });
            deferral.Complete();
        }

        public void modifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text == "")
            {
                var i = new MessageDialog("Please enter your username!").ShowAsync();
                return;
            }
            if (Password.Password == "")
            {
                var i = new MessageDialog("Please enter your password!").ShowAsync();
                return;
            }
            if (Password.Password.Length < 8)
            {
                var i = new MessageDialog("Your password must longer than 8 characters!").ShowAsync();
                return;
            }
            if (Phone.Text.Length != 11)
            {
                var i = new MessageDialog("Your phone must be 11 numbers!").ShowAsync();
                return;
            }
            if (Email.Text == "")
            {
                var i = new MessageDialog("Please enter your email!").ShowAsync();
                return;
            }
            Frame.Navigate(typeof(UserPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            if (user.Valid_usernameAsync(Phone.Text.ToString()))
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
