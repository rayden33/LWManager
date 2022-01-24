using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для ReturnOrder.xaml
    /// </summary>
    public partial class ReturnOrder : Window
    {
        private class OtherProductListItem
        {
            public int Order_product_id { get; set; }
            public string Name { get; set; }
            public int Return_count { get; set; }
            public int Count { get; set; }
            public int Price { get; set; }
        }
        public int ReturnTimeSpan { get; private set; }
        public List<ReturnProduct> ReturnProducts = new List<ReturnProduct>();
        public bool IsAllProductReturned = true;
        ApplicationContext dataBaseAC;
        MWViewContract selectedOrder;
        public ReturnOrder(ApplicationContext dbAp, MWViewContract tmpOrder)
        {
            InitializeComponent();


            dataBaseAC = dbAp;
            selectedOrder = tmpOrder;
            ///Disable all product buttons
            

            GetProducts();

        }

        private void GetProducts()
        {
            List<OrderProduct> orderProducts = new List<OrderProduct>();
            OrderProduct orderProduct = new OrderProduct();
            List<OtherProductListItem> otherProductListItems = new List<OtherProductListItem>();
            OtherProductListItem otherProductListItem = new OtherProductListItem();

            orderProducts = dataBaseAC.OrderProducts.Where(op => op.Order_id == selectedOrder.OrderId).ToList();
            foreach (OrderProduct op in orderProducts)
            {
                otherProductListItem = new OtherProductListItem();
                otherProductListItem.Order_product_id = op.Id;
                otherProductListItem.Name = dataBaseAC.Products.Find(op.Product_id).Name;
                otherProductListItem.Count = op.Count;
                otherProductListItem.Return_count = op.Count;
                otherProductListItem.Price = op.Count * op.Price;
                otherProductListItems.Add(otherProductListItem);
            }
            otherProductListBox.ItemsSource = otherProductListItems;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ReturnTimeSpan = (Int32)(returnDatePicker.SelectedDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            ReturnProduct tmpReturnProduct;
            OrderProduct tmpOrderProduct;
            if (otherProductListBox.Items.Count > 0)
            {
                foreach(OtherProductListItem opl in otherProductListBox.Items)
                {
                    tmpReturnProduct = new ReturnProduct();
                    tmpOrderProduct = dataBaseAC.OrderProducts.Find(opl.Order_product_id);
                    tmpReturnProduct.Order_id = tmpOrderProduct.Order_id;
                    tmpReturnProduct.Product_id = tmpOrderProduct.Product_id;
                    if (opl.Return_count != tmpOrderProduct.Count)
                        IsAllProductReturned = false;
                    tmpReturnProduct.Count = opl.Return_count;
                    tmpReturnProduct.Price = tmpOrderProduct.Price * opl.Count;
                    ReturnProducts.Add(tmpReturnProduct);
                }
            }
            DialogResult = true;
        }

        private void otherProductListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (otherProductListBox.SelectedItem == null)
                return;
            OtherProductListItem otherProductListItem = otherProductListBox.SelectedItem as OtherProductListItem;
            OrderProduct tmpOrderProdcut = dataBaseAC.OrderProducts.Find(otherProductListItem.Order_product_id);
            ReturnProductDetails returnProductDetails = new ReturnProductDetails(tmpOrderProdcut, dataBaseAC);

            if (returnProductDetails.ShowDialog() == true)
            {
                otherProductListItem.Return_count = returnProductDetails.ReturnOrderProductCount;
                otherProductListBox.SelectedItem = otherProductListItem;
                otherProductListBox.Items.Refresh();
            }
        }
    }
}
