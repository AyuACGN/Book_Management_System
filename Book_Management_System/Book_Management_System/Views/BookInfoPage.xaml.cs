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
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

/// <summary>
/// 该页面用于
/// 1.修改图书信息
/// 2.增加、删除图书
/// </summary>

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookInfoPage : Page
    {
        DataTransferManager dataTransferManager;

        public BookInfoPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;

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
            if (ViewModel.SelectedItem != null)
            {
                createButton.Content = "Update";
                title.Text = ViewModel.SelectedItem.name;
                description.Text = ViewModel.SelectedItem.description;
                date.Date = ViewModel.SelectedItem.datetime;
                pic.Source = new BitmapImage(new Uri(ViewModel.SelectedItem.imagepath));
                var conn = new SQLiteConnection("BMS.db");
                using (var statement = conn.Prepare("SELECT Name,Description,Date,ImagePath FROM BookItem WHERE Name = ?"))
                {
                    statement.Bind(1, title.Text);
                    statement.Step();
                    title.Text = (string)statement[0];
                    description.Text = (string)statement[1];
                    pic.Source = new BitmapImage(new Uri((string)statement[3]));
                    date.Date = Convert.ToDateTime((string)statement[2]).Date;
                }

                path = this.ViewModel.SelectedItem.imagepath;
            }
            else
            {
                pic.Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));
                createButton.Content = "Create";
            }
        }
        string path;

        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem == null)
            {
                string temp = "";
                if (date.Date < DateTime.Now.Date)
                {
                    temp += "Your Date is wrong!\n";
                }
                if (title.Text == "")
                {
                    temp += "The title can't be empty!\n";
                }
                if (description.Text == "")
                {
                    temp += "The Details can't be empty!\n";
                }
                if (temp != "")
                {
                    var i = new MessageDialog(temp).ShowAsync();
                }
                else if (temp == "")
                {
                    var i = new MessageDialog("Create success!").ShowAsync();
                    ViewModel.AddBook(title.Text, description.Text, date.Date.DateTime, path);
                    Frame.Navigate(typeof(AdminPage), ViewModel);
                }
            }
            else if (ViewModel.SelectedItem != null)
            {
                var i = new MessageDialog("Update success!").ShowAsync();
                ViewModel.UpdateBook(title.Text, description.Text, date.Date.DateTime, path);
                Frame.Navigate(typeof(AdminPage), ViewModel);
            }
        } // completed

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveBook(ViewModel.SelectedItem);
                
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }


        private void shareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            description.Text = "";
            date.Date = DateTime.Now.Date;
            pic.Source = new BitmapImage(new Uri(path));

            Frame.Navigate(typeof(AdminPage), ViewModel);
        }
        
        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            path = "ms-appx:///Assets/" + file.Name;
            if (file != null)
            {
                IRandomAccessStream ir = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bi = new BitmapImage();
                await bi.SetSourceAsync(ir);
                pic.Source = bi;
            }
        }
    }
}
