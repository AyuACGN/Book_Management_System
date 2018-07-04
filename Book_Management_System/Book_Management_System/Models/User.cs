using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Book_Management_System.Models
{
    class User
    {
        // 每个用户有一个单独的ID，以数字存储
        // ID用于检索用户
        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        private string _username;
        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        private string _passward;
        public string passward
        {
            get
            {
                return _passward;
            }
            set
            {
                _passward = value;
            }
        }

        private string _phone;
        public string phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
            }
        }

        private string _email;
        public string email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
            }
        }

        private List<string> _books;

        public List<string> books
        {
            get
            {
                return _books;
            }
            set
            {
                _books = value;
            }
        }

        // 管理员权限默认
        // 用户注册时自动设置为Reader
        private enum Authority { Reader, Administrator}
        private string _authority;
        public string authority
        {
            get
            {
                return _authority;
            }
            set
            {
                _authority = value;
            }
        }

        public User(string name, string pass, string phone, string email)
        {
            this.username = name;
            this.passward = pass;
            this.phone = phone;
            this.email = email;
            this.books = new List<string>();
        }

        public bool Valid_usernameAsync(string username)
        {
            if (username == "")
            {
                return false;
            }
            return true;
        }

        public bool valid_passwardAsync(string passward)
        {
            if (passward == "")
            {
                return false;
            }
            if (passward.Length < 8)
            {
                return false;
            }
            return true;
        }

        public bool valid_phoneAsync(string phone)
        {
            if (phone.Length != 11)
            {
                return false;
            }
            return Regex.IsMatch(phone, @"^1[34578]+\d{1}((-)?\d{4}){2}$");
        }

        public bool valid_emailAsync(string email)
        {
            if (email == "")
            {
                return false;
            }
            return Regex.IsMatch(email, @"^[A-Za-z]+([-.]\w+)*@\w+([-.]\w+)*\.[a-z]{2,3}$");
        }
    }
}
