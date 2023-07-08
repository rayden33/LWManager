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
    /// Логика взаимодействия для EditLeaseContract.xaml
    /// </summary>
    public partial class EditLeaseContract : Window
    {

        public LeaseContract LeaseContract { get; private set; }
        public ReturnedLeaseContract ReturnedLeaseContract { get; private set; }

        ApplicationContext dataBaseAC;

        private string _prevText = "";
        private string _currentSuggestion = "";
        private string _currentText = "";
        private int _selectionStart;
        private int _selectionLength;
        private List<string> _addressSuggestions = new List<string>();
        public EditLeaseContract(LeaseContract lc, ApplicationContext dbAC)
        {
            InitializeComponent();

            LeaseContract = lc;
            dataBaseAC = dbAC;

            contractIdTxtBox.Text = LeaseContract.Id.ToString();
            deliveryAmountTxtBox.Text = LeaseContract.Delivery_amount.ToString();
            deliveryAddressTxtBox.Text = LeaseContract.Delivery_address.ToString();
            createDatePicker.SelectedDate = new DateTime(1970, 1, 1).AddSeconds(LeaseContract.Create_datetime);


            //Height = SystemParameters.PrimaryScreenHeight / 1.5;
            //Width = SystemParameters.PrimaryScreenWidth / 3.84;

            
        }
        public EditLeaseContract(ReturnedLeaseContract lc, ApplicationContext dbAC)
        {
            InitializeComponent();

            ReturnedLeaseContract = lc;
            dataBaseAC = dbAC;

            contractIdTxtBox.Text = ReturnedLeaseContract.Id.ToString();
            deliveryAmountTxtBox.Text = ReturnedLeaseContract.Delivery_amount.ToString();
            deliveryAddressTxtBox.Text = ReturnedLeaseContract.Delivery_address.ToString();
            createDatePicker.SelectedDate = new DateTime(1970, 1, 1).AddSeconds(ReturnedLeaseContract.Create_datetime);


            //Height = SystemParameters.PrimaryScreenHeight / 1.5;
            //Width = SystemParameters.PrimaryScreenWidth / 3.84;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(LeaseContract != null)
            {
                LeaseContract.Delivery_address = deliveryAddressTxtBox.Text;
                LeaseContract.Delivery_amount = Convert.ToInt32(deliveryAmountTxtBox.Text);
                LeaseContract.Create_datetime = (Int32)(createDatePicker.SelectedDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                LeaseContract.Used_days = (usedDayChBox.IsChecked == true ? 0 : 1);
                DialogResult = true;
            }
            else
            {
                ReturnedLeaseContract.Delivery_address = deliveryAddressTxtBox.Text;
                ReturnedLeaseContract.Delivery_amount = Convert.ToInt32(deliveryAmountTxtBox.Text);
                ReturnedLeaseContract.Create_datetime = (Int32)(createDatePicker.SelectedDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                ReturnedLeaseContract.Used_days = (usedDayChBox.IsChecked == true ? 0 : 1);
                DialogResult = true;
            }
            
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string tmpTxt = deliveryAddressTxtBox.Text;
            if (tmpTxt.Length > 0 && tmpTxt != _currentSuggestion && _prevText != tmpTxt)
            {
                _currentSuggestion = _addressSuggestions.FirstOrDefault(x => x.StartsWith(tmpTxt));
                if (_currentSuggestion != null)
                {
                    _currentText = _currentSuggestion;
                    _selectionStart = tmpTxt.Length;
                    _selectionLength = _currentSuggestion.Length - tmpTxt.Length;

                    deliveryAddressTxtBox.Text = _currentText;
                    deliveryAddressTxtBox.Select(_selectionStart, _selectionLength);
                }
            }
            _prevText = tmpTxt;

        }
    }
}
