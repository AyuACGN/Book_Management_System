using Book_Management_System.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
using Windows.UI.Xaml.Media.Imaging;
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
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnShareDateRequested);

            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/mainpage.png", UriKind.Absolute))
            };
            gd_backimage.Background = imageBrush;
        }

        ViewModels.ManagementViewModels ViewModel;

        async void OnShareDateRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(ViewModel.SelectedItem.imagepath));
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
        
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            dynamic ori = e.OriginalSource;
            ViewModel.SelectedItem = (Book)ori.DataContext;
            ViewModel.RemoveBook(ViewModel.SelectedItem);
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
            var conn = new SQLiteConnection("BMS.db");
            using (var statement = conn.Prepare("SELECT Name,Description,Date,ImagePath FROM BookItem WHERE Name = ?"))
            {
                statement.Bind(1, Search.QueryText);
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append((string)statement[0]);
                    DateTime time = DateTime.Parse((string)statement[2]);
                    this.ViewModel.SelectedItem = new Models.Book((string)statement[0], (string)statement[1], time, (string)statement[3]);
                }
                if (str.Length == 0)
                {
                    var i = new MessageDialog("No this book!").ShowAsync();
                }
                else
                {
                    Frame.Navigate(typeof(BookPage), this.ViewModel);
                }
            }
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

    public class DateToStringConverter : IValueConverter
    {
        // Define the Convert method to convert a DateTime value to 
        // a month string.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            string path = (string)value;
            BitmapImage image = new BitmapImage(new Uri(path));
            return image;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
