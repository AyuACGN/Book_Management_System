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
        
        public Book(string name, string description, DateTime date, string imagePath)
        {
            this.id = Guid.NewGuid().ToString();
            this.name = name;
            this.description = description;
            this.datetime = date;
            this.imagePath = imagePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string imagePath;

        public string imagepath
        {
            get
            {
                return imagePath;
            }
            set
            {
                imagePath = value;
            }
        }
        
        private int _status;

        public int status
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
