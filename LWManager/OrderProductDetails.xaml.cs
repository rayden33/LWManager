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
    /// Логика взаимодействия для OrderProducts.xaml
    /// </summary>
    public partial class OrderProductDetails : Window
    {

        ApplicationContext dataBaseAC;
        List<ProductReference> productReferences;
        Product[] subProducts = new Product[10];
        public List<OrderProduct> OrderProducts { get; private set; }

        int[] productCounts = new int[10];
        Product product;
        OrderProduct tmpOrderProd = new OrderProduct();

        public OrderProductDetails(ApplicationContext dbAC, OrderProduct orderProduct)
        {
            InitializeComponent();
            dataBaseAC = dbAC;
            product = dataBaseAC.Products.Find(orderProduct.Product_id);
            productReferences = dataBaseAC.ProductReferences.Where(p => p.Product_id == product.Id).ToList();
            int j = 0;
            foreach (var prodRef in productReferences)
            {
                j++;
                //subProducts[prodRef.Ref_product_id] = dataBaseAC.Products.Find(prodRef.Ref_product_id);
                subProducts[j] = dataBaseAC.Products.Find(prodRef.Ref_product_id);
            }
            OrderProducts = new List<OrderProduct>();
            OrderProducts.Add(orderProduct);

            fillLabels();
        }

        void fillLabels()
        {

            if (productReferences.Count > 0)
            {
                productNameLbl.Content = product.Name;
                productCountTxtBox.Text = product.Count.ToString();
                productPriceTxtBox.Text = product.Price.ToString();
                
                if (subProductCountTxtBox1.Text.Length == 0)
                    subProductCountTxtBox1.Text = subProducts[1].Count.ToString();
                if (subProductCountTxtBox2.Text.Length == 0)
                    subProductCountTxtBox2.Text = subProducts[2].Count.ToString();
                if (subProductCountTxtBox3.Text.Length == 0)
                    subProductCountTxtBox3.Text = subProducts[3].Count.ToString();

            }
            else
            {
                productNameLbl.Content = product.Name;
                productCountTxtBox.Text = product.Count.ToString();
                productPriceTxtBox.Text = product.Price.ToString();
                subProductsSP.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int productCount = Convert.ToInt32(productCountTxtBox.Text);
            int productPrice = Convert.ToInt32(productPriceTxtBox.Text);
            if (productReferences.Count > 0)
            {
                int subProductCount1 = Convert.ToInt32(subProductCountTxtBox1.Text);
                int subProductCount2 = Convert.ToInt32(subProductCountTxtBox2.Text);
                int subProductCount3 = Convert.ToInt32(subProductCountTxtBox3.Text);

                
                if (WrongCounts(product, productCount)
                    || WrongCounts(subProducts[1], subProductCount1)
                    || WrongCounts(subProducts[2], subProductCount2)
                    || WrongCounts(subProducts[3], subProductCount3))
                {
                    return;
                }

                productCounts[1] = subProductCount1;
                productCounts[2] = subProductCount2;
                productCounts[3] = subProductCount3;

                
                OrderProducts[0].Count = productCount;
                OrderProducts[0].Price = productPrice;

                tmpOrderProd.Order_id = OrderProducts[0].Order_id;


                for (int i = 1; i <= 3; i++)
                {
                    tmpOrderProd = new OrderProduct();
                    tmpOrderProd.Order_id = OrderProducts[0].Order_id;
                    tmpOrderProd.Product_id = subProducts[i].Id;
                    tmpOrderProd.Count = productCounts[i];
                    tmpOrderProd.Price = 0;                                         ///// For ignore sub products prices

                    OrderProducts.Add(tmpOrderProd);
                }
            }
            else
            {
                if (WrongCounts(product, productCount))
                {
                    return;
                }

                OrderProducts[0].Count = productCount;
                OrderProducts[0].Price = productPrice;
            }
            this.DialogResult = true;
        }

        private bool WrongCounts(Product tmpProd, int productCount)
        {
            if (tmpProd.Count < productCount)
            {
                MessageBox.Show("Недостаточно " + tmpProd.Name + " Склад:" + tmpProd.Count);
                return true;
            }
            else
                return false;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void productCountTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productCountTxtBox.Text.Length == 0 
                || subProductCountTxtBox1 == null 
                || subProductCountTxtBox2 == null 
                || subProductCountTxtBox3 == null
                || productReferences.Count == 0)
                return;
            int productCount = Convert.ToInt32(productCountTxtBox.Text);
            subProductCountTxtBox1.Text = (productCount * productReferences[0].Ref_product_count).ToString();
            subProductCountTxtBox2.Text = (productCount * productReferences[1].Ref_product_count).ToString();
            subProductCountTxtBox3.Text = (productCount * productReferences[2].Ref_product_count).ToString();
        }

    }
}
