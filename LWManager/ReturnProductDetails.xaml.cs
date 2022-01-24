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
    /// Логика взаимодействия для ReturnProductDetails.xaml
    /// </summary>
    public partial class ReturnProductDetails : Window
    {
        public OrderProduct OrderProduct { get; private set; }
        ApplicationContext dataBaseAC;
        public int ReturnOrderProductCount;
        public ReturnProductDetails(OrderProduct orderProduct, ApplicationContext ac)
        {
            InitializeComponent();
            OrderProduct = orderProduct;
            dataBaseAC = ac;
            productNameLbl.Content = dataBaseAC.Products.Find(OrderProduct.Product_id).Name;
            returnedProductCountTxtBox.Text = OrderProduct.Count.ToString();
            orderProductCountTxtBox.Text = OrderProduct.Count.ToString();
            ReturnOrderProductCount = OrderProduct.Count;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(returnedProductCountTxtBox.Text) > Convert.ToInt32(orderProductCountTxtBox.Text))
            {
                MessageBox.Show("Неправильное значение");
                return;
            }
            //OrderProduct.Count = Convert.ToInt32(returnedProductCountTxtBox.Text);
            ReturnOrderProductCount = Convert.ToInt32(returnedProductCountTxtBox.Text);
            DialogResult = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }
    }
}
