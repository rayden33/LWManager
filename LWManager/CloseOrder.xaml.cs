using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LWManager
{
    /// <summary>
    /// Логика взаимодействия для CloseOrder.xaml
    /// </summary>
    public partial class CloseOrder : Window
    {
        public int CloseTimeSpan { get; private set; }
        public CloseOrder()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CloseTimeSpan = (Int32)(closedDatePicker.SelectedDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            DialogResult = true;
        }
    }
}
