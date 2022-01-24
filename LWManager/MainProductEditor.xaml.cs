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
    /// Логика взаимодействия для MainProductEditor.xaml
    /// </summary>
    public partial class MainProductEditor : Window
    {
        ApplicationContext dataBaseAC;
        List<ProductReference> productReferences;
        Product[] subProducts = new Product[10];

        int[] productCounts = new int[10];
        Product product;
        public Product Product { get; set; }
        public List<Product> SubProducts { get; set; }
        public MainProductEditor(ApplicationContext dbAC, Product editProduct)
        {
            InitializeComponent();
            dataBaseAC = dbAC;
            product = dataBaseAC.Products.Find(editProduct.Id);
            productReferences = dataBaseAC.ProductReferences.Where(p => p.Product_id == product.Id).ToList();
            int j = 0;
            foreach (var prodRef in productReferences)
            {
                j++;
                //subProducts[prodRef.Ref_product_id] = dataBaseAC.Products.Find(prodRef.Ref_product_id);
                subProducts[j] = dataBaseAC.Products.Find(prodRef.Ref_product_id);
            }

            fillLabels();
        }

        void fillLabels()
        {

            if (productReferences.Count > 0)
            {
                productNameLbl.Content = product.Name;
                productCountTxtBox.Text = product.Count.ToString();
                productPriceTxtBox.Text = product.Price.ToString();

                subProductCountTxtBox1.Text = subProducts[1].Count.ToString();
                subProductCountTxtBox2.Text = subProducts[2].Count.ToString();
                subProductCountTxtBox3.Text = subProducts[3].Count.ToString();

                subProductLabelTxtBox1.Content = subProducts[1].Name.Substring(0, subProducts[1].Name.Length-6);
                subProductLabelTxtBox2.Content = subProducts[2].Name.Substring(0, subProducts[2].Name.Length - 6);
                subProductLabelTxtBox3.Content = subProducts[3].Name.Substring(0, subProducts[3].Name.Length - 6);

            }
            else
            {
                productNameLbl.Content = product.Name;
                productCountTxtBox.Text = product.Count.ToString();
                productPriceTxtBox.Text = product.Price.ToString();
                subProductsSP.Visibility = Visibility.Hidden;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void productCountTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*if (productCountTxtBox.Text.Length == 0
                || subProductCountTxtBox1 == null
                || subProductCountTxtBox2 == null
                || subProductCountTxtBox3 == null
                || productReferences.Count == 0)
                return;
            int productCount = Convert.ToInt32(productCountTxtBox.Text);
            subProductCountTxtBox1.Text = (productCount * productReferences[0].Ref_product_count).ToString();
            subProductCountTxtBox2.Text = (productCount * productReferences[1].Ref_product_count).ToString();
            subProductCountTxtBox3.Text = (productCount * productReferences[2].Ref_product_count).ToString();*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            product.Count = Convert.ToInt32(productCountTxtBox.Text);
            product.Price = Convert.ToInt32(productPriceTxtBox.Text);
            if (productReferences.Count > 0)
            {
                subProducts[1].Count = Convert.ToInt32(subProductCountTxtBox1.Text);
                subProducts[2].Count = Convert.ToInt32(subProductCountTxtBox2.Text);
                subProducts[3].Count = Convert.ToInt32(subProductCountTxtBox3.Text);

            }


            Product = product;
            SubProducts = new List<Product>();
            for(int i = 1; i <= productReferences.Count; i++)
            {
                SubProducts.Add(subProducts[i]);
            }
            this.DialogResult = true;
        }
    }
}
