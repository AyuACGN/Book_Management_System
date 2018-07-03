using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        public bool Valid_usernameAsync(string username)
        {
            if (username == "")
            {
                // var i = new MessageDialog("Please input your username!").ShowAsync();
                return false;
            }
            return true;
        }

        public bool valid_passwardAsync(string passward)
        {
            if (passward == "")
            {
                // var i = new MessageDialog("Please input your passward!").ShowAsync();
                return false;
            }
            if (passward.Length < 8)
            {
                // var i = new MessageDialog("Passward must longer than 8 characters!").ShowAsync();
                return false;
            }
            return true;
        }

        public bool valid_phoneAsync(string phone)
        {
            if (phone.Length != 11)
            {
                // var i = new MessageDialog("Please input you 11 characters phone number!").ShowAsync();
                return false;
            }
            return true;
        }

        public bool valid_emailAsync(string email)
        {
            if (email == "")
            {
                // var i = new MessageDialog("Please input your email!").ShowAsync();
                return false;
            }
            return true;
        }
    }
}
