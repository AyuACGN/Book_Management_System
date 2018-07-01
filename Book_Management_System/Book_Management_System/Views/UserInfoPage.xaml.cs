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

        public string user;

        public UserInfoPage()
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
            UserName.Text = user;
        }

        private void modifyButton_Click(object sender, RoutedEventArgs e)
        {
            var conn = new SQLiteConnection("BookManageSystem.db");
            using (var statement = conn.Prepare("UPDATE UserList SET Password = ? WHERE Name = ?"))
            {
                statement.Bind(1, Password.Text);
                statement.Bind(2, user);
                statement.Step();
                var i = new MessageDialog("Success!").ShowAsync();
            }
        }
    }
}
