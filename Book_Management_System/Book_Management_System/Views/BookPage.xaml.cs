using SQLitePCL;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

/// <summary>
/// 该页面展示数据库中的所有图书
/// </summary>

namespace Book_Management_System.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookPage : Page
    {
        public BookPage()
        {
            this.InitializeComponent();
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/mainpage.png", UriKind.Absolute))
            };
            gd_backimage.Background = imageBrush;
        }

        ViewModels.ManagementViewModels ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.ManagementViewModels))
            {
                this.ViewModel = (ViewModels.ManagementViewModels)(e.Parameter);
            }
            if (ViewModel.SelectedItem != null)
            {
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
            }
            else
            {
                return;
            }
        }

        public void returnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), this.ViewModel);
        }
    }
}
