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
using Windows.UI.Xaml.Media.Imaging;

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

        User user = new User("", "", "", "");

        private ViewModels.ManagementViewModels ViewModel { get; set; }

        public UserInfoPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            
            UserName.PlaceholderText = "Original username: " + user.username;
            Password.PlaceholderText = "Enter your new password";
            Phone.PlaceholderText = "Original phone: " + user.phone;
            Email.PlaceholderText = "Original email: " + user.email;

            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/mainpage.png", UriKind.Absolute))
            };
            gd_.Background = imageBrush;
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = (ViewModels.ManagementViewModels)e.Parameter;
            UserName.Text = ViewModel.User;
            var conn = new SQLiteConnection("BMS.db");
            using (var statement = conn.Prepare("SELECT Password,Phone,Email FROM UserList WHERE Name = ?"))
            {
                statement.Bind(1, ViewModel.User);
                statement.Step();
                Password.Password = (string)statement[0];
                Phone.Text = (string)statement[1];
                Email.Text = (string)statement[2];
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

        public void ModifyButton_Click(object sender, RoutedEventArgs e)
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

            var conn = new SQLiteConnection("BMS.db");
            using (var statement = conn.Prepare("UPDATE UserList SET Password = ?,Phone = ?, Email = ? WHERE Name = ?"))
            {
                statement.Bind(1, Password.Password);
                statement.Bind(2, Phone.Text);
                statement.Bind(3, Email.Text);
                statement.Bind(4, ViewModel.User);
                statement.Step();
                var i = new MessageDialog("Success!").ShowAsync();
                Frame.Navigate(typeof(UserPage), ViewModel);
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
