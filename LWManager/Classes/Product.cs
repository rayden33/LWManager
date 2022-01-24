using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LWManager
{
    public class Product : INotifyPropertyChanged
    {
        private string name;
        private int price;
        private int count;
        private int reference_id;

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

        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("price");
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
        public int Reference_id
        {
            get { return reference_id; }
            set
            {
                reference_id = value;
                OnPropertyChanged("reference_id");
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
