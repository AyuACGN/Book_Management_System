using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

/// <summary>
/// 0.首页 MainPage
/// 1.注册 SignUpPage
/// 2.登录 SignInPage
/// 
/// 3.非功能 UserPage 用户登录以后进入个人主页UserPage，该页面包括所有选项
/// 4.非功能 BookPage 该页面包括数据库内所有图书
/// 
/// 5.修改个人信息 UserInfoPage
/// 6.图书增删改查 BookInfoPage
/// 7.查看历史记录 HistoryPage
/// 8.管理借还书页面 ManagePage
/// </summary>

namespace Book_Management_System.ViewModels
{
    class ManagementViewModels
    {
        private ObservableCollection<Models.Book> allbooks = new ObservableCollection<Models.Book>();
        public ObservableCollection<Models.Book> Allbooks { get { return this.allbooks} }

        private Models.Book selectedItem = default(Models.Book);
        private SQLiteConnection conn;

        public ManagementViewModels()
        {
            this.loadDatabase();
            this.initData();
        }

        private void loadDatabase()
        {
            conn = new SQLiteConnection("BookManagementSystem.db");
            string sql = @"CREATE TABLE IF NOT EXISTS
                             BookItem (Id VARCHAR(140) PRIMARY KEY,Title VARCHAR(140),Description VARCHAR(140),Date VARCHAR(140), BookNumber VARCHAR(140))";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }

            string usrList = @"CREATE TABLE IF NOT EXISTS
                             UserList (Name VARCHAR(140) PRIMARY KEY,Password VARCHAR(140),authority VARCHAR(140))";

            using (var statement1 = conn.Prepare(usrList))
            {
                statement1.Step();
            }

            string borrowbooklist = @"CREATE TABLE IF NOT EXISTS
                             BorrowHistory (UserName VARCHAR(140),BookName VARCHAR(140),Date VARCHAR(140))";
            using (var statement2 = conn.Prepare(borrowbooklist))
            {
                statement2.Step();
            }

            string returnbooklist = @"CREATE TABLE IF NOT EXISTS
                             ReturnHistory (UserName VARCHAR(140),BookName VARCHAR(140),Date VARCHAR(140))";
            using (var statement3 = conn.Prepare(returnbooklist))
            {
                statement3.Step();
            }
        } // 加载数据库，先链接数据库，如果表不存在，则创建新表

        private void initData()
        {
            var db = conn;
            try
            {
                using (var statement = db.Prepare("SELECT Id,Title,Description,Date,BookNumber FROM BookItem"))
                {
                    while (SQLiteResult.ROW ==statement.Step())
                    {
                        Models.Book newOne;
                        newOne = new Models.Book((string)statement[1], (string)statement[2], Convert.ToDateTime((string)statement[3]), (string)statement[4]);
                        newOne.id = (string)statement[0];
                        this.allbooks.Add(newOne);
                    }
                }
                using (var statement = db.Prepare("SELECT Name FROM UserList WHERE Name = ?"))
                {
                    statement.Bind(1, "admin");
                    if (SQLiteResult.ROW != statement.Step())
                    {
                        using (var ad = db.Prepare("INSERT INTO UserList (Name, Password, authority) VALUES (?, ?, ?)"))
                        {
                            ad.Bind(1, "admin");
                            ad.Bind(2, "admin");
                            ad.Bind(3, "2");
                            ad.Step();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        } // 初始化数据，读取当前本地存储的所有图书数据，加入一个管理员账号
    }
}
