using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LWManager
{
    public class Payment : INotifyPropertyChanged
    {
        private int amount;
        private int datetime;
        private int order_id;
        private string payment_type;

        public int Id { get; set; }

        public int Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                OnPropertyChanged("amount");
            }
        }

        public int Datetime
        {
            get { return datetime; }
            set
            {
                datetime = value;
                OnPropertyChanged("datetime");
            }
        }
        public int Order_id
        {
            get { return order_id; }
            set
            {
                order_id = value;
                OnPropertyChanged("order_id");
            }
        }
        public string Payment_type
        {
            get { return payment_type; }
            set
            {
                payment_type = value;
                OnPropertyChanged("payment_type");
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
