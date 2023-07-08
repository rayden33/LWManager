using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для MakePayment.xaml
    /// </summary>
    public partial class MakePayment : Window
    {
        public Payment Payment { get; private set; }
        public MakePayment(int orderId)
        {
            InitializeComponent();

            Payment = new Payment();
            Payment.Order_id = orderId;
            DataContext = Payment;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Payment.Payment_type = paymentTypeCombox.Text;
            Payment.Datetime = (Int32)(paymentDatePicker.SelectedDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (paymentTypeCombox.Text.ToLower().StartsWith("возврать"))
                Payment.Amount *= -1;
            DialogResult = true;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
