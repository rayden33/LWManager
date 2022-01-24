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
    /// Логика взаимодействия для LeaseContractEditor.xaml
    /// </summary>
    public partial class LeaseContractEditor : Window
    {
        private class OtherProductListItem
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public int Price { get; set; }
        }

        ApplicationContext dataBaseAC;
        Client selectedClient;

        List<OtherProductListItem> otherProductListItems = new List<OtherProductListItem>();
        public LeaseContract LeaseContract { get; private set; }
        public List<OrderProduct> NewOrderProducts { get; private set; }

        private OrderProduct tmpOrderProduct;
        private List<OrderProduct> tmpOrderProductsBLesa = new List<OrderProduct>();
        private List<OrderProduct> tmpOrderProductsMLesa = new List<OrderProduct>();
        private List<OrderProduct> tmpOrderProductsKoles = new List<OrderProduct>();
        private List<OrderProduct> tmpOrderProductsOther = new List<OrderProduct>();

        private string _prevText = "";
        private string _currentSuggestion = "";
        private string _currentText = "";

        private int _selectionStart;
        private int _selectionLength;
        private List<string> _addressSuggestions = new List<string>();

        public LeaseContractEditor( LeaseContract lc, ApplicationContext dbAC)
        {
            InitializeComponent();
            LeaseContract = lc;
            dataBaseAC = dbAC;
            int maxContId = 1;
            //LeaseContract.Contract_id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + 'N' + (dataBaseAC.LeaseContracts.Local.Max(l => l.Id) + 1).ToString();
            if (dataBaseAC.LeaseContracts.Count() > 0)
                maxContId = Math.Max((dataBaseAC.LeaseContracts.Max(c => c.Id) + 1), maxContId);
            if (dataBaseAC.ArchiveLeaseContracts.Count() > 0)
                maxContId = Math.Max((dataBaseAC.ArchiveLeaseContracts.Max(c => c.Order_id) + 1), maxContId);
            if (dataBaseAC.ReturnedLeaseContracts.Count() > 0)
                maxContId = Math.Max((dataBaseAC.ReturnedLeaseContracts.Max(c => c.Order_id) + 1), maxContId);

            LeaseContract.Contract_id = maxContId.ToString();

            this.DataContext = LeaseContract;


            //Height = SystemParameters.PrimaryScreenHeight / 1.5;
            //Width = SystemParameters.PrimaryScreenWidth / 3.84;

            foreach(LeaseContract leaseContract in dataBaseAC.LeaseContracts.ToList())
            {
                if (leaseContract.Delivery_address != null)
                    _addressSuggestions.Add(leaseContract.Delivery_address);
            }
            foreach (ArchiveLeaseContract archiveLeaseContract in dataBaseAC.ArchiveLeaseContracts.ToList())
            {
                if (archiveLeaseContract.Delivery_address != null)
                    _addressSuggestions.Add(archiveLeaseContract.Delivery_address);
            }
            foreach (ReturnedLeaseContract returnedLeaseContract in dataBaseAC.ReturnedLeaseContracts.ToList())
            {
                if (returnedLeaseContract.Delivery_address != null)
                    _addressSuggestions.Add(returnedLeaseContract.Delivery_address);
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Product tmpProduct = new Product();
            LeaseContract.Client_id = selectedClient.Id;

            LeaseContract.Price_per_day = 0;
            foreach (OrderProduct op in tmpOrderProductsBLesa)
            {
                LeaseContract.Price_per_day += (op.Count * op.Price);
                dataBaseAC.Products.Find(op.Product_id).Count -= op.Count;
            }
            foreach (OrderProduct op in tmpOrderProductsMLesa)
            {
                LeaseContract.Price_per_day += (op.Count * op.Price);
                dataBaseAC.Products.Find(op.Product_id).Count -= op.Count;
            }
            foreach (OrderProduct op in tmpOrderProductsKoles)
            {
                LeaseContract.Price_per_day += (op.Count * op.Price);
                dataBaseAC.Products.Find(op.Product_id).Count -= op.Count;
            }
            foreach (OrderProduct op in tmpOrderProductsOther)
            {
                LeaseContract.Price_per_day += (op.Count * op.Price);
                dataBaseAC.Products.Find(op.Product_id).Count -= op.Count;
            }
            //LeaseContract.Price_per_day += LeaseContract.Delivery_amount;

            LeaseContract.Used_days = (usedDayChBox.IsChecked == true ? 0 : 1);

            LeaseContract.Create_datetime = (Int32)(createDatePicker.SelectedDate.Value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            NewOrderProducts = new List<OrderProduct>();
            NewOrderProducts.AddRange(tmpOrderProductsBLesa);
            NewOrderProducts.AddRange(tmpOrderProductsMLesa);
            NewOrderProducts.AddRange(tmpOrderProductsKoles);
            NewOrderProducts.AddRange(tmpOrderProductsOther);

            //dataBaseAC.SaveChanges();

            this.DialogResult = true;
        }

        private void chooseClientBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientsList clientsList = new ClientsList(dataBaseAC);
            clientsList.isForSelectClient = true;
            if (clientsList.ShowDialog() == true)
            {
                selectedClient = clientsList.SelectedClient;
                clientSurnameTxtBox.Content = selectedClient.Surname;
                clientNameTxtBox.Content = selectedClient.Name;
                clientMiddleNameTxtBox.Content = selectedClient.Middle_name;
                clientPassNumberTxtBox.Content = selectedClient.Pass_number;
                clientPhoneNumberTxtBox.Content = selectedClient.Phone_number;
                clientPhoneNumber2TxtBox.Content = selectedClient.Phone_number2;
                productsStackPanel.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tmpOrderProduct = new OrderProduct();
            tmpOrderProduct.Product_id = 5;
            tmpOrderProduct.Order_id = 0;                                                                   /// Correct id get in MainWindow
            OrderProductDetails orderProductDetails = new OrderProductDetails(dataBaseAC, tmpOrderProduct);
            if (orderProductDetails.ShowDialog() == true)
            {
                //MessageBox.Show(orderProducts.productCounts[5].ToString());
                tmpOrderProductsBLesa = orderProductDetails.OrderProducts;
                bLesaCountLbl.Content = tmpOrderProductsBLesa[0].Count;
                int prodId;
                prodId = tmpOrderProductsBLesa[1].Product_id;
                bLesaSubNameLbl1.Content = dataBaseAC.Products.Find(prodId).Name;
                bLesaSubCountLbl1.Content = tmpOrderProductsBLesa[1].Count;
                prodId = tmpOrderProductsBLesa[2].Product_id;
                bLesaSubNameLbl2.Content = dataBaseAC.Products.Find(prodId).Name;
                bLesaSubCountLbl2.Content = tmpOrderProductsBLesa[2].Count;
                prodId = tmpOrderProductsBLesa[3].Product_id;
                bLesaSubNameLbl3.Content = dataBaseAC.Products.Find(prodId).Name;
                bLesaSubCountLbl3.Content = tmpOrderProductsBLesa[3].Count;
                AcceptBtn.IsEnabled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tmpOrderProduct = new OrderProduct();
            tmpOrderProduct.Product_id = 6;
            tmpOrderProduct.Order_id = 1;                                                                  /// Correct id get in MainWindow
            OrderProductDetails orderProductDetails = new OrderProductDetails(dataBaseAC, tmpOrderProduct);
            if (orderProductDetails.ShowDialog() == true)
            {
                //MessageBox.Show(orderProductDetails.productCounts[6].ToString());
                tmpOrderProductsMLesa = orderProductDetails.OrderProducts;
                mLesaCountLbl.Content = tmpOrderProductsMLesa[0].Count;
                int prodId;
                prodId = tmpOrderProductsMLesa[1].Product_id;
                mLesaSubNameLbl1.Content = dataBaseAC.Products.Find(prodId).Name;
                mLesaSubCountLbl1.Content = tmpOrderProductsMLesa[1].Count;
                prodId = tmpOrderProductsMLesa[2].Product_id;
                mLesaSubNameLbl2.Content = dataBaseAC.Products.Find(prodId).Name;
                mLesaSubCountLbl2.Content = tmpOrderProductsMLesa[2].Count;
                prodId = tmpOrderProductsMLesa[3].Product_id;
                mLesaSubNameLbl3.Content = dataBaseAC.Products.Find(prodId).Name;
                mLesaSubCountLbl3.Content = tmpOrderProductsMLesa[3].Count;
                AcceptBtn.IsEnabled = true;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OtherProductListItem otherProductListItem = new OtherProductListItem();
            ProductList productList = new ProductList(dataBaseAC, true);
            if (productList.ShowDialog() == true)
            {
                tmpOrderProduct = new OrderProduct();
                tmpOrderProduct.Product_id = productList.SelectedProduct.Id;
                tmpOrderProduct.Order_id = 1;                                                                   /// Correct id get in MainWindow
                OrderProductDetails orderProductDetails = new OrderProductDetails(dataBaseAC, tmpOrderProduct);
                if (orderProductDetails.ShowDialog() == true)
                {
                    otherProductListItem.Name = dataBaseAC.Products.Find(orderProductDetails.OrderProducts[0].Product_id).Name;
                    otherProductListItem.Count = orderProductDetails.OrderProducts[0].Count;
                    otherProductListItem.Price = orderProductDetails.OrderProducts[0].Count * orderProductDetails.OrderProducts[0].Price;
                    otherProductListItems.Add(otherProductListItem);
                    tmpOrderProductsOther.Add(orderProductDetails.OrderProducts[0]);
                    otherProductListBox.ItemsSource = otherProductListItems;
                    otherProductListBox.Items.Refresh();
                    AcceptBtn.IsEnabled = true;
                }


                
            }


            
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            
        }

        private void createClientBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientEditor clientEditor = new ClientEditor(new Client(), dataBaseAC);
            if (clientEditor.ShowDialog() == true)
            {
                Client client = clientEditor.Client;
                dataBaseAC.Clients.Add(client);
                dataBaseAC.SaveChanges();

                selectedClient = client;
                clientSurnameTxtBox.Content = selectedClient.Surname;
                clientNameTxtBox.Content = selectedClient.Name;
                clientMiddleNameTxtBox.Content = selectedClient.Middle_name;
                clientPassNumberTxtBox.Content = selectedClient.Pass_number;
                clientPhoneNumberTxtBox.Content = selectedClient.Phone_number;
                productsStackPanel.IsEnabled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //// for formating
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tmpOrderProduct = new OrderProduct();
            tmpOrderProduct.Product_id = 4;
            tmpOrderProduct.Order_id = 1;
            OrderProductDetails orderProductDetails = new OrderProductDetails(dataBaseAC, tmpOrderProduct);
            if (orderProductDetails.ShowDialog() == true)
            {
                //MessageBox.Show(orderProductDetails.productCounts[4].ToString());
                tmpOrderProductsKoles = orderProductDetails.OrderProducts;
                kolesikCountLbl.Content = tmpOrderProductsKoles[0].Count;
                AcceptBtn.IsEnabled = true;
            }
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
        /*private void CalculateTotalAmount()
{
int total = Convert.ToInt32(totalAmountLbl.Content);
total = LeaseContract.Delivery_amount;
foreach()
}*/
    }
}
