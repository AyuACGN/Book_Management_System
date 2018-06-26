using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Management_System.Models
{
    class Book
    {
        // 每本书有唯一的ID
        // 这个ID应该可以和读者ID重复
        // 这个ID用来检索图书
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

        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _author;
        public string author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
            }
        }

        private string _publisher;
        public string publisher
        {
            get
            {
                return _publisher;
            }
            set
            {
                _publisher = value;
            }
        }

        private string _isbn;
        public string isbn
        {
            get
            {
                return _isbn;
            }
            set
            {
                _isbn = value;
            }
        }

        private string _description;
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        // 新增图书时默认为Returned状态
        // 图书借出时更改为Borrowed状态
        // 图书归还时更改为Returned状态
        // 对图书进行操作是需要判断图书处于什么状态
        public enum Status { Borrowed, Returned }
        private Status _status;
        public Status status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
    }
}
