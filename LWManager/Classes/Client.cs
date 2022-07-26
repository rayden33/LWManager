using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LWManager
{
    public class Client : INotifyPropertyChanged
    {
        private string name;
        private string surname;
        private string middle_name;
        private string pass_number;
        private string phone_number;
        private string phone_number2;
        private string address;
        private int last_order_datetime;
        private int is_blocked;

        public int Id { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("name");
            }
        }

        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged("surname");
            }
        }
        public string Middle_name
        {
            get { return middle_name; }
            set
            {
                middle_name = value;
                OnPropertyChanged("middle_name");
            }
        }
        public string Pass_number
        {
            get { return pass_number; }
            set
            {
                pass_number = value;
                OnPropertyChanged("pass_number");
            }
        }
        public string Phone_number
        {
            get { return phone_number; }
            set
            {
                phone_number = value;
                OnPropertyChanged("phone_number");
            }
        }
        public string Phone_number2
        {
            get { return phone_number2; }
            set
            {
                phone_number2 = value;
                OnPropertyChanged("phone_number2");
            }
        }
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("address");
            }
        }
        public int Last_order_datetime
        {
            get { return last_order_datetime; }
            set
            {
                last_order_datetime = value;
                OnPropertyChanged("last_order_datetime");
            }
        }
        public int Is_blocked
        {
            get { return is_blocked; }
            set
            {
                is_blocked = value;
                OnPropertyChanged("is_blocked");
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
