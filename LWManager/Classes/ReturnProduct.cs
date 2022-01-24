using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LWManager
{
    public class ReturnProduct : INotifyPropertyChanged
    {
        private int order_id;
        private int product_id;
        private int count;
        private int price;

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

        public int Product_id
        {
            get { return product_id; }
            set
            {
                product_id = value;
                OnPropertyChanged("product_id");
            }
        }
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged("count");
            }
        }
        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("price");
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
