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
        public SignInPage()
        {
            this.InitializeComponent();
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

        }
    }
}
