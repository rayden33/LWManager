using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LWManager
{
    public class ProductReference : INotifyPropertyChanged
    {
        private int product_id;
        private int ref_product_id;
        private int ref_product_count;

        public int Id { get; set; }

        public int Product_id
        {
            get { return product_id; }
            set
            {
                product_id = value;
                OnPropertyChanged("product_id");
            }
        }

        public int Ref_product_id
        {
            get { return ref_product_id; }
            set
            {
                ref_product_id = value;
                OnPropertyChanged("ref_product_id");
            }
        }
        public int Ref_product_count
        {
            get { return ref_product_count; }
            set
            {
                ref_product_count = value;
                OnPropertyChanged("ref_product_count");
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
