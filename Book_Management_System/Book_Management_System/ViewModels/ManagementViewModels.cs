using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
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
        public ObservableCollection<Models.Book> Allbooks { get { return this.allbooks; } }

        private Models.Book selectedItem = default(Models.Book);
        private SQLiteConnection conn;

        public Models.Book SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public ManagementViewModels()
        {
            this.loadDatabase();
            this.initData();
        }

        private void loadDatabase()
        {
            conn = new SQLiteConnection("BMS.db");
            string sql = @"CREATE TABLE IF NOT EXISTS
                             BookItem (Id VARCHAR(140) PRIMARY KEY,Name VARCHAR(140),Description VARCHAR(140),Date VARCHAR(140), ImagePath VARCHAR(140), Status VARCHAR(140))";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }

            string usrList = @"CREATE TABLE IF NOT EXISTS
                             UserList (Name VARCHAR(140) PRIMARY KEY,Password VARCHAR(140),authority VARCHAR(140),Phone VARCHAR(140), Email VARCHAR(140))";

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
                // 初始化数据，读取当前本地存储的所有图书数据
                using (var statement = db.Prepare("SELECT Id,Name,Description,Date,ImagePath FROM BookItem"))
                {
                    while (SQLiteResult.ROW ==statement.Step())
                    {
                        Models.Book newOne;
                        newOne = new Models.Book((string)statement[1], (string)statement[2], Convert.ToDateTime((string)statement[3]), (string)statement[4]);
                        newOne.id = (string)statement[0];
                        this.allbooks.Add(newOne);
                    }
                }

                // 加入管理员账号
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
        }

        public int login(string username, string password)
        {
            var db = conn;

            // 检查用户是否存在
            using (var statement = conn.Prepare("SELECT Name FROM UserList WHERE Name = ?"))
            {
                statement.Bind(1, username);
                if (SQLiteResult.ROW != statement.Step())
                {
                    var i = new MessageDialog("This account has not been signed up!").ShowAsync();
                    return 0;
                }
            }

            // 检查用户名密码是否匹配，检测用户的权限
            using (var statement = conn.Prepare("SELECT authority FROM UserList WHERE Name = ? AND Password = ?"))
            {
                statement.Bind(1, username);
                statement.Bind(2, password);
                if (SQLiteResult.ROW == statement.Step())
                {
                    int a = int.Parse((string)statement[0]);
                    if (a == 1)
                    {
                        return 1;
                    }
                    else if (a == 2)
                    {
                        return 2;
                    }
                }
                else
                {
                    var i = new MessageDialog("Wrong password!").ShowAsync();
                    return 0;
                }
            }
            return 0;
        } // 登陆函数

        public int AddUser(string username, string password, string phone, string email)
        {
            var db = conn;

            // 检查用户名是否被注册
            using (var statement = conn.Prepare("SELECT Name FROM UserList WHERE Name = ?"))
            {
                statement.Bind(1, username);
                if (SQLiteResult.ROW == statement.Step())
                {
                    var i = new MessageDialog("This account has been signed up!").ShowAsync();
                    return 0;
                }
            }
            // 将创建的用户信息写入数据库
            try
            {
                using (var userItem = db.Prepare("INSERT INTO UserList (Name,Password,Authority,Phone,Email) VALUES (?, ?, ?, ?, ?)"))
                {
                    userItem.Bind(1, username);
                    userItem.Bind(2, password);
                    userItem.Bind(3, "1");
                    userItem.Bind(4, phone);
                    userItem.Bind(5, email);
                    userItem.Step();
                    var i = new MessageDialog("Success!").ShowAsync();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
            return 0;
        }

        // 加入新的书目
        public void AddBook(string name, string description, DateTime a, string imagePath)
        {
            Models.Book newOne;
            newOne = new Models.Book(name, description, a, imagePath);
            this.allbooks.Add(newOne);
            

            using (var statement = conn.Prepare("SELECT Id FROM BookItem WHERE Name = ?"))
            {
                statement.Bind(1, name);
                if (SQLiteResult.ROW == statement.Step())
                {
                    var i = new MessageDialog("This book has already existed!").ShowAsync();
                    return;
                }
            }

            var db = conn;
            try
            {
                using (var todoItem = db.Prepare("INSERT INTO BookItem (Id, Name, Description, Date, ImagePath, Status) VALUES (?, ?, ?, ?, ?, ?)"))
                {
                    todoItem.Bind(1, newOne.id);
                    todoItem.Bind(2, name);
                    todoItem.Bind(3, description);
                    todoItem.Bind(4, a.ToString());
                    todoItem.Bind(5, imagePath);
                    todoItem.Bind(6, 1.ToString());
                    todoItem.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        // 删除书目
        public void RemoveBook(Models.Book item)
        {
            using (var statement = conn.Prepare("DELETE FROM BookItem WHERE Id = ?"))
            {
                statement.Bind(1, item.id);
                statement.Step();
            }
            this.Allbooks.Remove(item);
            this.selectedItem = null;
        }

        // 更新书本信息
        public void UpdateBook(string name, string description, DateTime datetime, string imagePath)
        {
            if (this.selectedItem != null)
            {
                using (var todoitem = conn.Prepare("UPDATE BookItem SET Name = ?, Description = ?, Date = ?, ImagePath = ? WHERE Id = ?"))
                {
                    todoitem.Bind(1, name);
                    todoitem.Bind(2, description);
                    todoitem.Bind(3, datetime.ToString());
                    todoitem.Bind(4, imagePath);
                    todoitem.Bind(5, selectedItem.id);
                    todoitem.Step();
                }
            }
            this.selectedItem.name = name;
            this.selectedItem.description = description;
            this.selectedItem.datetime = datetime;
            this.selectedItem.imagepath = imagePath;

            this.selectedItem = null;
        }

        // 查询书本信息
        public void QueryBook(string text)
        {
            using (var statement = conn.Prepare("SELECT Name,Description FROM BookItem WHERE Name = ? OR Description = ? OR Date = ?"))
            {
                statement.Bind(1, text);
                statement.Bind(2, text);
                statement.Bind(3, text);
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append(" Name:");
                    str.Append((string)statement[0]);
                    str.Append("  Description:");
                    str.Append((string)statement[1]);
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

        public void AddBorrowRecord(string username, string bookname, DateTime a)
        {
            var db = conn;
            try
            {
                using (var borrowItem = db.Prepare("INSERT INTO BorrowHistory (UserName, BookName, Date) VALUES (?, ?, ?)"))
                {
                    borrowItem.Bind(1, username);
                    borrowItem.Bind(2, bookname);
                    borrowItem.Bind(3, a.ToString());
                    borrowItem.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        } // 添加借书记录

        public void AddReturnRecord(string username, string bookname, DateTime a)
        {
            var db = conn;
            try
            {
                using (var returnItem = db.Prepare("INSERT INTO ReturnHistory (UserName, BookName, Date) VALUES (?, ?, ?)"))
                {
                    returnItem.Bind(1, username);
                    returnItem.Bind(2, bookname);
                    returnItem.Bind(3, a.ToString());
                    returnItem.Step();
                }
            }
            catch (Exception ex)
            {
                var i = new MessageDialog(ex.ToString()).ShowAsync();
            }
        } // 添加还书记录

        public void borrowBook(string bookname, string username, DateTime a)
        {
            using (var sss = conn.Prepare("SELECT UserName FROM BorrowHistory WHERE UserName = ? AND BookName = ?"))
            {
                sss.Bind(1, username);
                sss.Bind(2, bookname);
                if (SQLiteResult.ROW == sss.Step())
                {
                    var i = new MessageDialog("You have borrowed this book!").ShowAsync();
                    return;
                }
            }
            using (var sss = conn.Prepare("SELECT Name FROM UserList WHERE Name = ?"))
            {
                sss.Bind(1, username);
                if (SQLiteResult.ROW != sss.Step())
                {
                    var i = new MessageDialog("No this user!").ShowAsync();
                    return;
                }
            }
            using (var statement = conn.Prepare("SELECT Name,Description,Date,Status FROM BookItem WHERE Name = ?"))
            {
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                statement.Bind(1, bookname);
                if (SQLiteResult.ROW == statement.Step())
                {
                    if (int.Parse((string)statement[3]) == 0)
                    {
                        var i = new MessageDialog("This book is out of loan").ShowAsync();
                    }
                    else
                    {
                        AddBorrowRecord(username, bookname, a);
                        var i = new MessageDialog("Borrow sucess!").ShowAsync();
                        using (var sta = conn.Prepare("UPDATE BookItem SET Status = ? WHERE Name = ?"))
                        {
                            sta.Bind(1, 0.ToString());
                            sta.Bind(2, bookname);
                            sta.Step();
                        }
                    }
                }
                else
                {
                    var i = new MessageDialog("There is no this book").ShowAsync();
                }
            }
        } // 完整借书流程

        public void returnBook(string bookname, string username, DateTime a)
        {
            using (var sss = conn.Prepare("SELECT Name FROM UserList WHERE Name = ?"))
            {
                sss.Bind(1, username);
                if (SQLiteResult.ROW != sss.Step())
                {
                    var i = new MessageDialog("No this user!").ShowAsync();
                    return;
                }
            }

            using (var sss = conn.Prepare("SELECT Name FROM BookItem WHERE Name = ?"))
            {
                sss.Bind(1, bookname);
                if (SQLiteResult.ROW != sss.Step())
                {
                    var i = new MessageDialog("There is no theis book!").ShowAsync();
                    return;
                }
            }

            using (var statement = conn.Prepare("SELECT Name,Description,Date,ImagePath FROM BookItem WHERE Name = ?"))
            {
                StringBuilder str = new StringBuilder();
                str.Length = 0;
                statement.Bind(1, bookname);
                if (SQLiteResult.ROW == statement.Step())
                {
                    using (var std = conn.Prepare("SELECT BookName FROM BorrowHistory WHERE BookName = ? And UserName = ? "))
                    {
                        std.Bind(1, bookname);
                        std.Bind(2, username);
                        if (SQLiteResult.ROW != std.Step())
                        {
                            var i = new MessageDialog("You have not borrowed this book!").ShowAsync();
                            return;
                        }
                        else
                        {
                            using (var s = conn.Prepare("SELECT Status FROM BookItem WHERE Name = ?"))
                            {
                                s.Bind(1, bookname);
                                if (SQLiteResult.ROW == s.Step())
                                {
                                    if (int.Parse((string)s[0]) == 1)
                                    {
                                        var im = new MessageDialog("This book has been returned").ShowAsync();
                                    }
                                    else
                                    {
                                        AddReturnRecord(username, bookname, a);
                                        var ie = new MessageDialog("return sucess!").ShowAsync();
                                        using (var sta = conn.Prepare("UPDATE BookItem SET Status = ? WHERE Name = ?"))
                                        {
                                            sta.Bind(1, 1.ToString());
                                            sta.Bind(2, bookname);
                                            sta.Step();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } // 完整还书流程

        public string AdminHistory(string bookname)
        {
            using (var statement = conn.Prepare("SELECT Name FROM BookItem WHERE Name = ?"))
            {
                statement.Bind(1, bookname);
                if (SQLiteResult.ROW != statement.Step())
                {
                    var i = new MessageDialog("There is no this book!").ShowAsync();
                    return "error";
                }
            }

            StringBuilder str = new StringBuilder();
            str.Length = 0;
            str.Append("BorrowHistory\n");

            using (var statement = conn.Prepare("SELECT BookName,UserName,Date FROM BorrowHistory WHERE BookName = ?"))
            {
                statement.Bind(1, bookname);
                str.Append("UserName      Date");
                str.Append("\r\n");
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append((string)statement[1]);
                    str.Append("             ");
                    str.Append((string)statement[2]);
                    str.Append("\r\n");
                }
            }

            str.Append("ReturnHistory\n");

            using (var statement = conn.Prepare("SELECT BookName,UserName,Date FROM ReturnHistory WHERE BookName = ?"))
            {
                statement.Bind(1, bookname);
                str.Append("UserName      Date");
                str.Append("\r\n");
                while (SQLiteResult.ROW == statement.Step())
                {
                    str.Append((string)statement[1]);
                    str.Append("             ");
                    str.Append((string)statement[2]);
                    str.Append("\r\n");
                }
                return str.ToString();
            }
        } // 管理员查看书本借阅历史
    }
}
