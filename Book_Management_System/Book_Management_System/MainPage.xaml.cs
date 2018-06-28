using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.ApplicationModel.DataTransfer;
using Book_Management_System.Views;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Book_Management_System
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DataTransferManager dataTransferManager;

        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.ManagementViewModels();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnShareDateRequested);
        }

        ViewModels.ManagementViewModels ViewModel { get; set; }

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
            if (e.Parameter.GetType() == typeof(ViewModels.ManagementViewModels))
            {
                this.ViewModel = (ViewModels.ManagementViewModels)(e.Parameter);
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = "";
            if (UserName.Text == null)
            {
                temp += "The user name can't be empty!\n";
            }
            if (Password.Password == null)
            {
                temp += "The password can't be empty!\n";
            }
            if (temp != "")
            {
                var i = new MessageDialog(temp).ShowAsync();
            }
            else if (temp == "")
            {
                ViewModel.AddUser(UserName.Text, Password.Password);
            }
        }

        private void signinButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = "";
            if (UserName.Text == null)
            {
                temp += "The user name can't be empty!\n";
            }
            if (Password.Password == null)
            {
                temp += "The password can't be empty!\n";
            }
            if (temp != "")
            {
                var i = new MessageDialog(temp).ShowAsync();
            }
            else if (temp == "")
            {
                int a = ViewModel.login(UserName.Text, Password.Password);
                if (a == 2)
                {
                    Frame.Navigate(typeof(AdminPage), ViewModel);
                }
                else if (a == 1)
                {
                    Frame.Navigate(typeof(UserPage), UserName.Text);
                }
            }
        }
    }
}
