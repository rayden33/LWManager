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
    /// Логика взаимодействия для ClientEditor.xaml
    /// </summary>
    public partial class ClientEditor : Window
    {
        public Client Client { get; private set; }
        public bool isEditMode = false;
        private ApplicationContext dataBaseAC;
        public ClientEditor(Client client, ApplicationContext dbac)
        {
            InitializeComponent();
            Client = client;
            dataBaseAC = dbac;
            DataContext = Client;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (!isCorrectPhoneNumber()) return;
            if (!isCorrectPhoneNumber2()) return;
            if (isAlreadyRegistrated()) return;
            DialogResult = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void phoneNumberTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            isCorrectPhoneNumber();
        }

        private bool isCorrectPhoneNumber()
        {
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace("(", "");
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace(")", "");
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace(" ", "");
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace("-", "");
            if (phoneNumberTxtBox.Text.Length == 9)
            {
                string tmpStr = "(" + phoneNumberTxtBox.Text.Substring(0, 2) + ") ";
                tmpStr += phoneNumberTxtBox.Text.Substring(2, 3) + "-";
                tmpStr += phoneNumberTxtBox.Text.Substring(5, 2) + "-";
                tmpStr += phoneNumberTxtBox.Text.Substring(7, 2);
                phoneNumberTxtBox.Text = tmpStr;
                return true;
            }
            else
            {
                if (phoneNumberTxtBox.Text.Length == 0) return true;
                MessageBox.Show("Исправьте или оставьте пустым номер телефона (Пример: 991234569)");
                phoneNumberTxtBox.Text = "";
                return false;
            }
        }

        private bool isCorrectPhoneNumber2()
        {
            phoneNumber2TxtBox.Text = phoneNumber2TxtBox.Text.Replace("(", "");
            phoneNumber2TxtBox.Text = phoneNumber2TxtBox.Text.Replace(")", "");
            phoneNumber2TxtBox.Text = phoneNumber2TxtBox.Text.Replace(" ", "");
            phoneNumber2TxtBox.Text = phoneNumber2TxtBox.Text.Replace("-", "");
            if (phoneNumber2TxtBox.Text.Length == 9)
            {
                string tmpStr = "(" + phoneNumber2TxtBox.Text.Substring(0, 2) + ") ";
                tmpStr += phoneNumber2TxtBox.Text.Substring(2, 3) + "-";
                tmpStr += phoneNumber2TxtBox.Text.Substring(5, 2) + "-";
                tmpStr += phoneNumber2TxtBox.Text.Substring(7, 2);
                phoneNumber2TxtBox.Text = tmpStr;
                return true;
            }
            else
            {
                if (phoneNumber2TxtBox.Text.Length == 0) return true;
                MessageBox.Show("Исправьте или оставьте пустым второй номер телефона (Пример: 991234569)");
                phoneNumber2TxtBox.Text = "";
                return false;
            }
        }

        private bool isAlreadyRegistrated()
        {
            Client tmpClient = dataBaseAC.Clients.Where(c => c.Phone_number == phoneNumberTxtBox.Text).FirstOrDefault();
            if (((dataBaseAC.Clients.Where(c => c.Pass_number == clientPassNumberTextBox.Text).FirstOrDefault() != null && clientPassNumberTextBox.Text.Length != 0)
                || (dataBaseAC.Clients.Where(c => c.Phone_number == phoneNumberTxtBox.Text).FirstOrDefault() != null && phoneNumberTxtBox.Text.Length != 0)
                || (dataBaseAC.Clients.Where(c => c.Phone_number == phoneNumber2TxtBox.Text).FirstOrDefault() != null && phoneNumber2TxtBox.Text.Length != 0)
                || (dataBaseAC.Clients.Where(c => c.Phone_number2 == phoneNumberTxtBox.Text).FirstOrDefault() != null && phoneNumberTxtBox.Text.Length != 0)
                || (dataBaseAC.Clients.Where(c => c.Phone_number2 == phoneNumber2TxtBox.Text).FirstOrDefault() != null) && phoneNumber2TxtBox.Text.Length != 0)
                && !isEditMode)
            {
                MessageBox.Show("Этот клиент уже есть в базе");
                return true;
            }
            else
                return false;
        }

        private void phoneNumber2TxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            isCorrectPhoneNumber2();
        }

        /*private void phoneNumberTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (phoneNumberTxtBox.Text == null)
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace("(","");
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace(")","");
            phoneNumberTxtBox.Text = phoneNumberTxtBox.Text.Replace(" ","");
            string tmpStr = "(" + phoneNumberTxtBox.Text.Substring(0, 2) + ") ";
            tmpStr += phoneNumberTxtBox.Text.Substring(2, 3) + "-";
            tmpStr += phoneNumberTxtBox.Text.Substring(5, 2) + "-";
            tmpStr += phoneNumberTxtBox.Text.Substring(7, 2);
            phoneNumberTxtBox.Text = tmpStr;
        }*/
    }
}
