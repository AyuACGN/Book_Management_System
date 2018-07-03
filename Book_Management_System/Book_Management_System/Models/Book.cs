using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Book_Management_System.Models
{
    class Book : INotifyPropertyChanged
    {

        public string id;

        private string _title;

        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private DateTime _datetime;

        public DateTime datetime
        {
            get
            {
                return _datetime;
            }
            set
            {
                _datetime = value;
                OnPropertyChanged();
            }
        }
        
        private string _bookNumber;

        public string bookNumber
        {
            get
            {
                return _bookNumber;
            }
            set
            {
                _bookNumber = value;
                OnPropertyChanged();
            }
        }

        public Book(string title, string description, DateTime date, string booknumber)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title;
            this.description = description;
            this.datetime = date;
            this.bookNumber = booknumber;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
