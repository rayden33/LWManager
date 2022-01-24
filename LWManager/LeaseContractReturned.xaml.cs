using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для LeaseContractReturned.xaml
    /// </summary>
    public partial class LeaseContractReturned : Window
    {
        ApplicationContext dataBaseAC;
        List<MWViewContract> viewContracts;
        MWViewContract tempViewContract;
        Client tempClient;
        DateTime tempDateTime;
        TimeSpan tempTimeSpan;
        public LeaseContractReturned(ApplicationContext dbap)
        {
            InitializeComponent();
            dataBaseAC = dbap;

            LoadFromSQLiteDB();
            GetDbToDataGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            LeaseContractViewer leaseContractViewer = new LeaseContractViewer(dataBaseAC, mWViewContract);
            leaseContractViewer.ShowDialog();
        }

        void GetDbToDataGrid()
        {
            int i = 0;
            viewContracts = new List<MWViewContract>();

            foreach (ReturnedLeaseContract contract in dataBaseAC.ReturnedLeaseContracts)
            {
                i++;
                tempViewContract = new MWViewContract();
                tempViewContract.OrderId = contract.Order_id;
                tempViewContract.RowNumber = i;

                tempClient = dataBaseAC.Clients.FirstOrDefault(client => client.Id == contract.Client_id);
                tempViewContract.FISH = (tempClient.Surname + " " + tempClient.Name + " " + tempClient.Middle_name);

                tempDateTime = UnixTimeStampToDateTime(contract.Create_datetime);
                tempViewContract.CreationDateTime = tempDateTime.ToShortDateString();

                if (contract.Return_datetime == 0)
                    tempTimeSpan = DateTime.Now - tempDateTime;
                else
                    tempTimeSpan = UnixTimeStampToDateTime(contract.Return_datetime) - tempDateTime;
                tempViewContract.UsedDays = $"{tempTimeSpan.Days} " + ((contract.Used_days > 0) ? ($"(+{contract.Used_days})") : (""));
                int usedDaysTotal = tempTimeSpan.Days + contract.Used_days;

                if (dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Order_id && op.Product_id == 5).FirstOrDefault() != null)
                    tempViewContract.BLease = dataBaseAC.ReturnProducts.Where(rt => rt.Order_id == contract.Order_id && rt.Product_id == 5).FirstOrDefault().Count.ToString() + " из " + dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Order_id && op.Product_id == 5).FirstOrDefault().Count.ToString();
                if (dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Order_id && op.Product_id == 6).FirstOrDefault() != null)
                    tempViewContract.LLease = dataBaseAC.ReturnProducts.Where(rt => rt.Order_id == contract.Order_id && rt.Product_id == 6).FirstOrDefault().Count.ToString() + " из " + dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Order_id && op.Product_id == 6).FirstOrDefault().Count.ToString();
                if (dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Order_id && op.Product_id == 4).FirstOrDefault() != null)
                    tempViewContract.Wheel = dataBaseAC.ReturnProducts.Where(rt => rt.Order_id == contract.Order_id && rt.Product_id == 4).FirstOrDefault().Count.ToString() + " из " + dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Order_id && op.Product_id == 4).FirstOrDefault().Count.ToString();
                tempViewContract.Phone = tempClient.Phone_number;
                tempViewContract.DeliveryPrice = contract.Delivery_amount;
                tempViewContract.DeliveryAddress = contract.Delivery_address;
                tempViewContract.PaidAmount = contract.Paid_amount;
                tempViewContract.Sum = contract.Paid_amount - (contract.Price_per_day * usedDaysTotal + contract.Delivery_amount);
                tempViewContract.OrderStatus = 1;
                viewContracts.Add(tempViewContract);

            }
            this.DataContext = viewContracts;


        }

        private void LoadFromSQLiteDB()
        {
            dataBaseAC = new ApplicationContext();
            dataBaseAC.ReturnedLeaseContracts.Load();
            dataBaseAC.Clients.Load();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            MakePayment makePayment = new MakePayment(mWViewContract.OrderId);
            if (makePayment.ShowDialog() == true)
            {
                Payment payment = makePayment.Payment;
                dataBaseAC.Payments.Add(payment);
                ReturnedLeaseContract returnedLeaseContract = dataBaseAC.ReturnedLeaseContracts.Where(r => r.Order_id == mWViewContract.OrderId).FirstOrDefault();
                returnedLeaseContract.Paid_amount += payment.Amount;
                dataBaseAC.Entry(returnedLeaseContract).State = EntityState.Modified;
                dataBaseAC.SaveChanges();
            }
            GetDbToDataGrid();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            closeOrder(mWViewContract);
        }

        private void closeOrder(MWViewContract mWViewContract)
        {
            if (mWViewContract.Sum < 0)
            {
                MessageBox.Show("Оплатите долг");
                return;
            }
            if (!isReturnedAllProducts(mWViewContract))
            {
                return;
            }
            CloseOrder closeOrder = new CloseOrder();
            if (closeOrder.ShowDialog() == true)
            {
                ReturnedLeaseContract returnedLeaseContract = dataBaseAC.ReturnedLeaseContracts.Where( rl => rl.Order_id == mWViewContract.OrderId).FirstOrDefault();
                ArchiveLeaseContract archiveLeaseContract = new ArchiveLeaseContract();
                archiveLeaseContract.Order_id = returnedLeaseContract.Order_id;
                archiveLeaseContract.Client_id = returnedLeaseContract.Client_id;
                archiveLeaseContract.Contract_id = returnedLeaseContract.Contract_id;
                archiveLeaseContract.Paid_amount = returnedLeaseContract.Paid_amount;
                archiveLeaseContract.Price_per_day = returnedLeaseContract.Price_per_day;
                archiveLeaseContract.Delivery_amount = returnedLeaseContract.Delivery_amount;
                archiveLeaseContract.Delivery_address = returnedLeaseContract.Delivery_address;
                archiveLeaseContract.Used_days = returnedLeaseContract.Used_days;
                archiveLeaseContract.Create_datetime = returnedLeaseContract.Create_datetime;
                archiveLeaseContract.Return_datetime = returnedLeaseContract.Return_datetime;
                archiveLeaseContract.Close_datetime = closeOrder.CloseTimeSpan;
                dataBaseAC.ArchiveLeaseContracts.Add(archiveLeaseContract);

                dataBaseAC.ReturnedLeaseContracts.Remove(returnedLeaseContract);
                dataBaseAC.SaveChanges();
                GetDbToDataGrid();
            }
        }
        private void returnOrder(MWViewContract mWViewContract)
        {
            //int orderId = dataBaseAC.ReturnedLeaseContracts.Where(rl => rl.Order_id == mWViewContract.OrderId).FirstOrDefault();
            List<OrderProduct> orderProducts = dataBaseAC.OrderProducts.Where(op => op.Order_id == mWViewContract.OrderId).ToList();
            foreach(OrderProduct orderProduct in orderProducts)
            {
                ReturnProduct returnProduct = new ReturnProduct();
                returnProduct = dataBaseAC.ReturnProducts.Where(rp => rp.Order_id == orderProduct.Order_id && rp.Product_id == orderProduct.Product_id).FirstOrDefault();
                int countDiff = orderProduct.Count - returnProduct.Count;
                dataBaseAC.Products.Find(orderProduct.Product_id).Count += countDiff;
                returnProduct.Count = orderProduct.Count;
                dataBaseAC.Entry(returnProduct).State = EntityState.Modified;
                dataBaseAC.SaveChanges();
            }
        }
        private bool isReturnedAllProducts(MWViewContract mWViewContract)
        {
            int orderId = mWViewContract.OrderId;
            List<OrderProduct> orderProducts = dataBaseAC.OrderProducts.Where(op => op.Order_id == orderId).ToList();
            foreach(OrderProduct orderProduct in orderProducts)
            {
                if (dataBaseAC.ReturnProducts.Where(rp => rp.Order_id == orderId && rp.Product_id == orderProduct.Product_id && rp.Count == orderProduct.Count).FirstOrDefault() == null)
                {
                    MessageBox.Show("Верните товар:" + dataBaseAC.Products.Find(orderProduct.Product_id).Name);
                    return false;
                }
            }
            return true;
        }

        private void leaseContractDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            if (MessageBox.Show("Точно хотите вернут все продукты?", "Вернут?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            returnOrder(mWViewContract);
        }
        private void AutoSizeWindowAndElements()
        {
            DataGrid DataGridForResize = leaseContractDataGrid;
            double DefaultPaddingPerColumn = 20;
            double FreeSpaceForPaddingPerColumn = 0;
            double SummOfWidthAllColumns = 0;
            foreach (DataGridColumn col in DataGridForResize.Columns)
            {
                SummOfWidthAllColumns += col.ActualWidth;
            }

            if (DataGridForResize.ActualWidth > (SummOfWidthAllColumns - 8))
                FreeSpaceForPaddingPerColumn = (DataGridForResize.ActualWidth - SummOfWidthAllColumns - 8) / (double)DataGridForResize.Columns.Count;
            else
                FreeSpaceForPaddingPerColumn = DefaultPaddingPerColumn;

            for (int i = 0; i < DataGridForResize.Columns.Count; i++)
            {
                DataGridForResize.Columns[i].Width = DataGridForResize.Columns[i].ActualWidth + FreeSpaceForPaddingPerColumn;
            }
        }

        private void leaseContractDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            AutoSizeWindowAndElements();
        }
    }
}
