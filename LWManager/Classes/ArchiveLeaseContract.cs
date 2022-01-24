using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LWManager
{
    public class ArchiveLeaseContract : INotifyPropertyChanged
    {
        private int order_id;
        private int client_id;
        private string contract_id;
        //private int product_group_id;
        private int paid_amount;
        private int price_per_day;
        private int delivery_amount;
        private string delivery_address;
        private int used_days;
        private int create_datetime;
        private int return_datetime;
        private int close_datetime;

        public int Id { get; set; }

        public int Order_id
        {
            get { return order_id; }
            set
            {
                order_id = value;
                OnPropertyChanged("order_id");
            }
        }

        public int Client_id
        {
            get { return client_id; }
            set
            {
                client_id = value;
                OnPropertyChanged("client_id");
            }
        }
        public string Contract_id
        {
            get { return contract_id; }
            set
            {
                contract_id = value;
                OnPropertyChanged("contract_id");
            }
        }
        //public int Product_group_id
        //{
        //    get { return product_group_id; }
        //    set
        //    {
        //        product_group_id = value;
        //        OnPropertyChanged("product_group_id");
        //    }
        //}
        public int Paid_amount
        {
            get { return paid_amount; }
            set
            {
                paid_amount = value;
                OnPropertyChanged("paid_amount");
            }
        }
        public int Price_per_day
        {
            get { return price_per_day; }
            set
            {
                price_per_day = value;
                OnPropertyChanged("price_per_day");
            }
        }
        public int Delivery_amount
        {
            get { return delivery_amount; }
            set
            {
                delivery_amount = value;
                OnPropertyChanged("delivery_amount");
            }
        }
        public string Delivery_address
        {
            get { return delivery_address; }
            set
            {
                delivery_address = value;
                OnPropertyChanged("delivery_address");
            }
        }
        public int Used_days
        {
            get { return used_days; }
            set
            {
                used_days = value;
                OnPropertyChanged("used_days");
            }
        }
        public int Create_datetime
        {
            get { return create_datetime; }
            set
            {
                create_datetime = value;
                OnPropertyChanged("create_datetime");
            }
        }
        public int Return_datetime
        {
            get { return return_datetime; }
            set
            {
                return_datetime = value;
                OnPropertyChanged("return_datetime");
            }
        }
        public int Close_datetime
        {
            get { return close_datetime; }
            set
            {
                close_datetime = value;
                OnPropertyChanged("close_datetime");
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
