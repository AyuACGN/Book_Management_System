using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Management_System.Models
{
    class History
    {
        // 该条记录的ID
        // 用于检索记录
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

        // 该条记录涉及的读者
        private int _readerID;
        public int readerID
        {
            get
            {
                return _readerID;
            }
            set
            {
                _readerID = value;
            }
        }

        // 该条记录涉及的图书
        private int _bookID;
        public int bookID
        {
            get
            {
                return _bookID;
            }
            set
            {
                _bookID = value;
            }
        }

        // 借书时间
        private DateTime _borrowTime;
        public DateTime borrowTime
        {
            get
            {
                return _borrowTime;
            }
            set
            {
                _borrowTime = value;
            }
        }

        // 还书时间
        private DateTime _returnTime;
        public DateTime returnTime
        {
            get
            {
                return _returnTime;
            }
            set
            {
                _returnTime = value;
            }
        }

        // 记录的类型，借书和还书
        public enum Status { Borrowed, Returned}
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
