using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

/// <summary>
/// 该页面为用户界面
/// 读者在该页面可以进行以下操作
/// 1.修改个人信息
/// 2.查询图书
/// 3.查询历史记录
/// 管理员可以在该页面进行以下操作
/// 1.图书操作（增删改查）
/// 2.图书借还
/// 3.查询历史记录
/// </summary>

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserPage : Page
    {
        DataTransferManager dataTransferManager;

        public string user;

        public UserPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;

            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnShareDateRequested);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            user = (string)e.Parameter;

        }

        private void Search_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            var conn = new SQLiteConnection("BookManageSystem.db");
            using (var statement = conn.Prepare("SELECT Title,Description,Date,BookNumber FROM TodoItem WHERE Title = ?"))
            {
                statement.Bind(1, Search.QueryText);
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append("Title:");
                    str.Append((string)statement[0]);
                    str.Append("  Description:");
                    str.Append((string)statement[1]);
                    str.Append("  Book Number:");
                    str.Append((string)statement[3]);
                    str.Append("\n");
                }
                if (str.Length == 0)
                {
                    var i = new MessageDialog("No result!").ShowAsync();
                }
                else
                {
                    var i = new MessageDialog(str.ToString()).ShowAsync();
                }
            }
        }

        private void searchHis_Click(object sender, RoutedEventArgs e)
        {
            var conn = new SQLiteConnection("BookManageSystem.db");
            int q = 0;
            using (var statement = conn.Prepare("SELECT BookName,Date FROM ReturnHistory WHERE UserName = ?"))
            {
                statement.Bind(1, user);
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                str.Append("The books have been returned:\n");
                str.Append("BookName             Date\n");
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append((string)statement[0]);
                    str.Append("                ");
                    str.Append((string)statement[1]);
                    str.Append("\n");
                }
                str.Append("\n\n\n");
                his.Text = str.ToString();
            }
            using (var statement = conn.Prepare("SELECT BookName,Date FROM BorrowHistory WHERE UserName = ?"))
            {
                statement.Bind(1, user);
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                str.Append("The books have been borrowed:\n");
                str.Append("BookName             Date\n");
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append((string)statement[0]);
                    str.Append("                ");
                    str.Append((string)statement[1]);
                    str.Append("\n");
                }
                his.Text += str.ToString();
            }
        }

        private void modify_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserInfoPage), user);
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }
    }
}
