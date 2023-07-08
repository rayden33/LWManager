using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Data.Entity;
using System.IO;

namespace LWManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ApplicationContext dataBaseAC;
        List<MWViewContract> viewContracts;
        MWViewContract tempViewContract;
        Client tempClient;
        DateTime tempDateTime;
        TimeSpan tempTimeSpan;

        public MainWindow()
        {
            InitializeComponent();

            LoadTmpInfo();

            /// <-- For datetime label
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            /// -->


            //this.DataContext = dataBaseAC.LeaseContracts.Local.ToBindingList();
            LoadFromSQLiteDB();
            GetDbToDataGrid();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLongTimeString() + "\n";
            lblTime.Content += DateTime.Now.ToShortDateString();
            
        }

        void GetDbToDataGrid()
        {
            int i = 0;
            viewContracts = new List<MWViewContract>();

            foreach (LeaseContract contract in dataBaseAC.LeaseContracts)
            {
                i++;
                tempViewContract = new MWViewContract();
                tempViewContract.OrderId = contract.Id;
                tempViewContract.RowNumber = i;

                tempClient = dataBaseAC.Clients.FirstOrDefault(client => client.Id == contract.Client_id);
                tempViewContract.FISH = (tempClient.Name + " " + tempClient.Surname);

                tempDateTime = UnixTimeStampToDateTime(contract.Create_datetime);
                tempViewContract.CreationDateTime = tempDateTime.ToShortDateString();

                if (contract.Return_datetime == 0)
                    tempTimeSpan = DateTime.Now - tempDateTime;
                else
                    tempTimeSpan = UnixTimeStampToDateTime(contract.Return_datetime) - tempDateTime;
                tempViewContract.UsedDays = $"{tempTimeSpan.Days + 1} " + ((contract.Used_days > 0) ? ("") : ("(-1)"));
                int usedDaysTotal = tempTimeSpan.Days + contract.Used_days;

                if (dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Id && op.Product_id == 5).FirstOrDefault() != null)
                    tempViewContract.BLease = dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Id && op.Product_id == 5).FirstOrDefault().Count.ToString();
                if (dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Id && op.Product_id == 6).FirstOrDefault() != null)
                    tempViewContract.LLease = dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Id && op.Product_id == 6).FirstOrDefault().Count.ToString();
                if (dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Id && op.Product_id == 4).FirstOrDefault() != null)
                    tempViewContract.Wheel = dataBaseAC.OrderProducts.Where(op => op.Order_id == contract.Id && op.Product_id == 4).FirstOrDefault().Count.ToString();
                tempViewContract.Phone = tempClient.Phone_number;
                tempViewContract.DeliveryPrice = contract.Delivery_amount;
                tempViewContract.DeliveryAddress = contract.Delivery_address;
                tempViewContract.PaidAmount = contract.Paid_amount;
                tempViewContract.Sum = contract.Paid_amount - (contract.Price_per_day * usedDaysTotal + contract.Delivery_amount);
                tempViewContract.IsDebtor = (tempViewContract.Sum < (-1 * Properties.Settings.Default.DebtLimit)) ? "1" : "0";
                switch(contract.Return_datetime)
                {
                    case 0: tempViewContract.OrderStatus = 0;
                        break;
                    default: tempViewContract.OrderStatus = 1;
                        break;
                }
                viewContracts.Add(tempViewContract);

            }
            this.DataContext = viewContracts;
        }

        private void LoadFromSQLiteDB()
        {
            dataBaseAC = new ApplicationContext();
            dataBaseAC.LeaseContracts.Load();
            dataBaseAC.Clients.Load();
            dataBaseAC.Products.Load();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Add new Lease contract
        /// </summary>
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (dataBaseAC.LeaseContracts.Count() == 0)
            //    dataBaseAC.LeaseContracts.Add(new LeaseContract() { Contract_id = "0", Client_id = 0 });
            LeaseContractEditor leaseContractEditor = new LeaseContractEditor(new LeaseContract(), dataBaseAC);
            if (leaseContractEditor.ShowDialog() == true)
            {
                LeaseContract leaseContract = leaseContractEditor.LeaseContract;
                dataBaseAC.LeaseContracts.Add(leaseContract);
                dataBaseAC.SaveChanges();

                foreach (OrderProduct op in leaseContractEditor.NewOrderProducts)
                {
                    op.Order_id = leaseContract.Id;
                    dataBaseAC.OrderProducts.Add(op);
                }

                dataBaseAC.SaveChanges();

            }

            GetDbToDataGrid();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClientsList clientsList = new ClientsList(dataBaseAC);
            clientsList.isForSelectClient = false;
            clientsList.ShowDialog();
            DataGridFormating();
        }

        /// Make payment
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            MakePayment makePayment = new MakePayment(mWViewContract.OrderId);
            if (makePayment.ShowDialog() == true)
            {
                Payment payment = makePayment.Payment;
                dataBaseAC.Payments.Add(payment);
                LeaseContract leaseContract =  dataBaseAC.LeaseContracts.Find(mWViewContract.OrderId);
                leaseContract.Paid_amount += payment.Amount;
                dataBaseAC.Entry(leaseContract).State = EntityState.Modified;
                dataBaseAC.SaveChanges();
            }
            GetDbToDataGrid();
        }

        /// <summary>
        /// Remove Lease contract
        /// </summary>
        
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;

            if (MessageBox.Show("Точно хотите удалить?", "Удалить?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            LeaseContract leaseContract = dataBaseAC.LeaseContracts.Find(mWViewContract.OrderId);
            List<OrderProduct> orderProducts = dataBaseAC.OrderProducts.Where(op => op.Order_id == mWViewContract.OrderId).ToList();
            foreach(OrderProduct orderProduct in orderProducts)
            {
                dataBaseAC.Products.Find(orderProduct.Product_id).Count += orderProduct.Count;
            }
            List<Payment> payments = dataBaseAC.Payments.Where(p => p.Order_id == mWViewContract.OrderId).ToList();
            dataBaseAC.LeaseContracts.Remove(leaseContract);
            dataBaseAC.OrderProducts.RemoveRange(orderProducts);
            dataBaseAC.Payments.RemoveRange(payments);
            dataBaseAC.SaveChanges();
            GetDbToDataGrid();


        }

        /// <summary>
        /// Close order
        /// </summary>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            
            returnOrder(mWViewContract);

            if (MessageBox.Show("Напечатать чек?", "Печать", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ReturnedLeaseContract returnedLeaseContract = dataBaseAC.ReturnedLeaseContracts.Where(p => p.Order_id == mWViewContract.OrderId).FirstOrDefault();
            
            if(returnedLeaseContract != null)
                PrintInvoiceWithWord(returnedLeaseContract);
        }

        private void PrintInvoiceWithWord(ReturnedLeaseContract contract)
        {

            /*Client client = dataBaseAC.Clients.Where(q => q.Id == contract.Client_id).FirstOrDefault();
            List<Payment> payments = dataBaseAC.Payments.Where(q => q.Order_id == contract.Order_id).ToList();
            // If using Professional version, put your serial key below.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            int paymentCount = payments.Count;

            DocumentModel document = DocumentModel.Load("Invoice.docx");

            // Template document contains 4 tables, each contains some set of information.
            Table[] tables = document.GetChildElements(true, ElementType.Table).Cast<Table>().ToArray();

            // First table contains invoice number and date.
            Table invoiceTable = tables[0];
            invoiceTable.Rows[0].Cells[1].Blocks.Add(new Paragraph(document, contract.Contract_id));
            invoiceTable.Rows[1].Cells[1].Blocks.Add(new Paragraph(document, $"{client.Name} {client.Surname}"));
            invoiceTable.Rows[2].Cells[1].Blocks.Add(new Paragraph(document, client.Phone_number.ToString()));

            // Second table contains customer data.
            Table customerTable = tables[1];
            customerTable.Rows[0].Cells[1].Blocks.Add(new Paragraph(document, contract.Delivery_address));
            customerTable.Rows[1].Cells[1].Blocks.Add(new Paragraph(document, contract.Delivery_amount.ToString()));
            customerTable.Rows[2].Cells[1].Blocks.Add(new Paragraph(document, UnixTimeStampToDateTime(contract.Create_datetime).ToString("d MMM yyyy HH:mm")));
            //customerTable.Rows[3].Cells[1].Blocks.Add(new Paragraph(document, "Joe Smith"));

            // Third table contains amount and prices, it only has one data row in the template document.
            // So, we'll dynamically add cloned rows for the rest of our data items.
            Table mainTable = tables[2];
            for (int i = 1; i < paymentCount; i++)
                mainTable.Rows.Insert(1, mainTable.Rows[1].Clone(true));

            int total = 0;
            int rowIndex = 0;
            foreach (Payment payment in payments)
            {
                mainTable.Rows[rowIndex].Cells[0].Blocks.Add(new Paragraph(document, UnixTimeStampToDateTime(payment.Datetime).ToString("d MMM yyyy HH:mm")));
                mainTable.Rows[rowIndex].Cells[1].Blocks.Add(new Paragraph(document, payment.Payment_type));
                mainTable.Rows[rowIndex].Cells[2].Blocks.Add(new Paragraph(document, payment.Amount.ToString("0.00")));
                rowIndex++;
                total += payment.Amount;
                //mainTable.Rows[rowIndex].Cells[3].Blocks.Add(new Paragraph(document, price.ToString("0.00")));
            }

            // Last cell in the last, total, row has some predefined formatting stored in an empty paragraph.
            // So, in this case instead of adding new paragraph we'll add our data into an existing paragraph.
            mainTable.Rows.Last().Cells[3].Blocks.Cast<Paragraph>(0).Content.LoadText(total.ToString("0.00"));

            // Fourth table contains notes.
            Table notesTable = tables[3];
            notesTable.Rows[1].Cells[0].Blocks.Add(new Paragraph(document, "Payment via check."));

            document.Save("Template Use.docx");*/
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ProductList productList = new ProductList(dataBaseAC, false);
            productList.ShowDialog();
        }

        private void leaseContractDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void returnOrder(MWViewContract mWViewContract)
        {
            ReturnOrder returnOrder = new ReturnOrder(dataBaseAC, mWViewContract);
            if (returnOrder.ShowDialog() == true)
            {
                LeaseContract leaseContract = dataBaseAC.LeaseContracts.Find(mWViewContract.OrderId);

                if (mWViewContract.Sum < 0 || !returnOrder.IsAllProductReturned)
                {
                    ReturnedLeaseContract returnedLeaseContract = new ReturnedLeaseContract();
                    returnedLeaseContract.Order_id = leaseContract.Id;
                    returnedLeaseContract.Client_id = leaseContract.Client_id;
                    returnedLeaseContract.Contract_id = leaseContract.Contract_id;
                    returnedLeaseContract.Paid_amount = leaseContract.Paid_amount;
                    returnedLeaseContract.Price_per_day = leaseContract.Price_per_day;
                    returnedLeaseContract.Delivery_amount = leaseContract.Delivery_amount;
                    returnedLeaseContract.Delivery_address = leaseContract.Delivery_address;
                    returnedLeaseContract.Used_days = leaseContract.Used_days;
                    returnedLeaseContract.Create_datetime = leaseContract.Create_datetime;
                    returnedLeaseContract.Return_datetime = returnOrder.ReturnTimeSpan;
                    returnedLeaseContract.Close_datetime = leaseContract.Close_datetime;
                    dataBaseAC.ReturnedLeaseContracts.Add(returnedLeaseContract);
                }
                else
                {
                    ArchiveLeaseContract archiveLeaseContract = new ArchiveLeaseContract();
                    archiveLeaseContract.Order_id = leaseContract.Id;
                    archiveLeaseContract.Client_id = leaseContract.Client_id;
                    archiveLeaseContract.Contract_id = leaseContract.Contract_id;
                    archiveLeaseContract.Paid_amount = leaseContract.Paid_amount;
                    archiveLeaseContract.Price_per_day = leaseContract.Price_per_day;
                    archiveLeaseContract.Delivery_amount = leaseContract.Delivery_amount;
                    archiveLeaseContract.Delivery_address = leaseContract.Delivery_address;
                    archiveLeaseContract.Used_days = leaseContract.Used_days;
                    archiveLeaseContract.Create_datetime = leaseContract.Create_datetime;
                    archiveLeaseContract.Return_datetime = returnOrder.ReturnTimeSpan;
                    archiveLeaseContract.Close_datetime = returnOrder.ReturnTimeSpan;
                    dataBaseAC.ArchiveLeaseContracts.Add(archiveLeaseContract);
                }

                foreach(ReturnProduct returnProduct in returnOrder.ReturnProducts)
                {
                    dataBaseAC.Products.Find(returnProduct.Product_id).Count += returnProduct.Count;
                }
                dataBaseAC.ReturnProducts.AddRange(returnOrder.ReturnProducts);

                dataBaseAC.LeaseContracts.Remove(leaseContract);
                dataBaseAC.SaveChanges();
                GetDbToDataGrid();
            }
        }



        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            LeaseContractArchive leaseContractArchive = new LeaseContractArchive(dataBaseAC);
            leaseContractArchive.ShowDialog();
        }

        private void leaseContractDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            
        }
        private void DataGridFormating()
        {
            /*MessageBox.Show(leaseContractDataGrid.Items.Count.ToString());
            foreach (MWViewContract mWViewContract in leaseContractDataGrid.Items)
            {
                
                
                if (mWViewContract.Sum < 0)
                    le
            }*/
        }


        /// <summary>
        /// ViewContract
        /// </summary>

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            LeaseContractViewer leaseContractViewer = new LeaseContractViewer(dataBaseAC, mWViewContract);
            leaseContractViewer.ShowDialog();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            LeaseContractReturned leaseContractReturned = new LeaseContractReturned(dataBaseAC);
            leaseContractReturned.ShowDialog();
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = searchTxtBox.Text;
            List<MWViewContract> searchResult = new List<MWViewContract>();
            GetDbToDataGrid();
            //viewContracts = dataBaseAC.ArchiveLeaseContracts.ToList();
            if (text == null || text == "")
            {
                GetDbToDataGrid();
                return;
            }

            foreach (MWViewContract tmpContract in viewContracts)
            {
                if (tmpContract.FISH != null && tmpContract.FISH.Contains(text))
                    searchResult.Add(tmpContract);
                else if (tmpContract.Phone != null && tmpContract.Phone.Contains(text))
                    searchResult.Add(tmpContract);
                else if (tmpContract.DeliveryAddress != null && tmpContract.DeliveryAddress.Contains(text))
                    searchResult.Add(tmpContract);
            }

            this.DataContext = searchResult;
        }


        private void AutoSizeWindowAndElements()
        {
            DataGrid DataGridForResize = leaseContractDataGrid;
            double DefaultPaddingPerColumn = 10;
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

        private void LeaseContractEditBtn(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;
            
            EditLeaseContract editLeaseContract = new EditLeaseContract(dataBaseAC.LeaseContracts.Where(l => l.Id == mWViewContract.OrderId).FirstOrDefault(), dataBaseAC);
            if(editLeaseContract.ShowDialog() == true)
            {
                dataBaseAC.Entry(editLeaseContract.LeaseContract).State = EntityState.Modified;
                dataBaseAC.SaveChanges();
            }

            GetDbToDataGrid();
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            MainStats mainStats = new MainStats(dataBaseAC);
            mainStats.Show();
        }

        private void LoadTmpInfo()
        {
            if(File.Exists(".\\~companyLogo.png"))
            {
                File.Copy(".\\~companyLogo.png", ".\\companyLogo.png", true);
                File.Delete(".\\~companyLogo.png");
            }
        }
    }
}
