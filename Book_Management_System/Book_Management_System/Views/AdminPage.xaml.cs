using Book_Management_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        DataTransferManager dataTransferManager;

        public AdminPage()
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

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.Book)(e.ClickedItem);
            Frame.Navigate(typeof(BookInfoPage), ViewModel);
            
        } // completed

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            
            this.Frame.Navigate(typeof(BookInfoPage), ViewModel);
            
        } // completed

        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.UpdateBook(_Title.Text, Details.Text, date.Date.DateTime, Booknumber.Text);
                _Title.Text = "";
                Details.Text = "";
                date.Date = DateTime.Now.Date;
                KeyButton.Content = "Create";
                
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
            else if (ViewModel.SelectedItem == null)
            {
                string temp = "";
                if (date.Date < DateTime.Now.Date)
                {
                    temp += "Your Date is wrong!\n";
                }
                if (_Title.Text == "")
                {
                    temp += "The title can't be empty!\n";
                }
                if (Details.Text == "")
                {
                    temp += "The Details can't be empty!\n";
                }
                if (temp != "")
                {
                    var i = new MessageDialog(temp).ShowAsync();
                }
                else if (temp == "")
                {
                    string title = _Title.Text;
                    string des = Details.Text;
                    DateTime d = date.Date.DateTime;
                    string bn = Booknumber.Text;
                    ViewModel.AddBook(title, des, d, bn);

                    var i = new MessageDialog("Success!").ShowAsync();
                }
            }
        } // completed

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _Title.Text = "";
            Details.Text = "";
            date.Date = DateTime.Now.Date;
        } // completed

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            ViewModel.SelectedItem = (Book)ori.DataContext;
            ViewModel.RemoveBook(ViewModel.SelectedItem);
            _Title.Text = "";
            Details.Text = "";
            date.Date = DateTime.Now.Date;
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            
            KeyButton.Content = "Create";
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            ViewModel.SelectedItem = (Book)ori.DataContext;
            DataTransferManager.ShowShareUI();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            ViewModel.SelectedItem = (Book)ori.DataContext;
            Frame.Navigate(typeof(BookInfoPage), ViewModel);
        }

        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            ViewModel.QueryBook(Search.QueryText);
        }

        private void ManageRequest_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ManagePage), ViewModel);
        }

        private void findhistory_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HistoryPage), ViewModel);
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), ViewModel);
        }
    }
}
