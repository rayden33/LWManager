using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;

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
                //tempViewContract.UsedDays = $"{tempTimeSpan.Days} " + ((contract.Used_days > 0) ? ($"(+{contract.Used_days})") : (""));
                tempViewContract.UsedDays = $"{tempTimeSpan.Days + 1} " + ((contract.Used_days > 0) ? ("") : ("(-1)"));
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
                dataBaseAC.Entry(returnedLeaseContract).State = EntityState.Unchanged;
                dataBaseAC.SaveChanges();
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;

            if (MessageBox.Show("Напечатать чек?", "Печать", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ReturnedLeaseContract returnedLeaseContract = dataBaseAC.ReturnedLeaseContracts.Where(p => p.Order_id == mWViewContract.OrderId).FirstOrDefault();

            if (returnedLeaseContract != null)
                PrintInvoiceWithWord(returnedLeaseContract);
        }

        private void PrintInvoiceWithWord(ReturnedLeaseContract contract)
        {

            Client client = dataBaseAC.Clients.Where(q => q.Id == contract.Client_id).FirstOrDefault();
            List<Payment> payments = dataBaseAC.Payments.Where(q => q.Order_id == contract.Order_id).ToList();
            List<OrderProduct> orderProducts = dataBaseAC.OrderProducts.Where(q => q.Order_id == contract.Order_id).ToList();

            int paymentCount = payments.Count;
            int usedDays = (UnixTimeStampToDateTime(contract.Return_datetime) - UnixTimeStampToDateTime(contract.Create_datetime)).Days;
            int shouldPay = contract.Price_per_day * (usedDays + contract.Used_days) + contract.Delivery_amount;
            int debt = shouldPay - contract.Paid_amount;
            string cellNumberFormat = "#,#";

            try
            {
/// Document initialization
                this.Cursor = Cursors.Wait;
                var xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlApp.DefaultSaveFormat = Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;
                if (xlApp != null)
                {
                    var xlWorkBook = xlApp.Workbooks.Add();
                    var sheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

/// Company info
    /// Company logo
                    if(File.Exists($".\\{Properties.Settings.Default.CompanyLogoImageName}"))
                    {
                        Microsoft.Office.Interop.Excel.Range oRange = (Microsoft.Office.Interop.Excel.Range)sheet.Cells[1, 3];
                        float Left = (float)((double)oRange.Left);
                        float Top = (float)((double)oRange.Top);
                        const float ImageSize = 60;
                        sheet.Shapes.AddPicture($"{AppDomain.CurrentDomain.BaseDirectory}\\{Properties.Settings.Default.CompanyLogoImageName}", MsoTriState.msoFalse, MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    }

/// Header text
                    int row = 3;
                    var range = sheet.Range[$"A{row}:H{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.MergeCells = true;
                    range.Font.Size = 24;
                    range.Value = "ЧЕК";

    /// First header table
                    sheet.Cells.Range[$"B{row+2}:G{row+4}"].Borders.LineStyle = 1;
        /// First row of header table
                    row += 2;
            /// Column left part
                    sheet.Cells[row, 2].Font.Bold = true;
                    sheet.Cells[row, 2].HorizontalAlignment = 4;
                    sheet.Cells[row, 2] = "Номер договора:";
                    sheet.Range[$"B{row}:C{row}"].MergeCells = true;
            /// Column right part
                    sheet.Cells[row, 4].HorizontalAlignment = 3;
                    sheet.Cells[row, 4] = $"{contract.Contract_id}";
                    sheet.Range[$"D{row}:G{row}"].MergeCells = true;
        /// Second row of header table
                    row++;
            /// Column left part
                    sheet.Cells[row, 2].Font.Bold = true;
                    sheet.Cells[row, 2].HorizontalAlignment = 4;
                    sheet.Cells[row, 2] = "Ф.И.О. заказчика:";
                    sheet.Range[$"B{row}:C{row}"].MergeCells = true;
            /// Column right part
                    sheet.Cells[row, 4].HorizontalAlignment = 3;
                    sheet.Cells[row, 4] = $"{client.Name} {client.Surname}";
                    sheet.Range[$"D{row}:G{row}"].MergeCells = true;
        /// Third row of header table
                    row++;
            /// Column left part
                    sheet.Cells[row, 2].Font.Bold = true;
                    sheet.Cells[row, 2].HorizontalAlignment = 4;
                    sheet.Cells[row, 2] = "Номер телефон заказчика:";
                    sheet.Range[$"B{row}:C{row}"].MergeCells = true;
            /// Column right part
                    sheet.Cells[row, 4].HorizontalAlignment = 3;
                    sheet.Cells[row, 4] = $"{client.Phone_number}";
                    sheet.Range[$"D{row}:G{row}"].MergeCells = true;

    /// Second header table
                    sheet.Cells.Range[$"B{row + 2}:G{row + 4}"].Borders.LineStyle = 1;
        /// First row of header table
                    row += 2;
            /// Column left part
                    sheet.Cells[row, 2].Font.Bold = true;
                    sheet.Cells[row, 2].HorizontalAlignment = 4;
                    sheet.Cells[row, 2] = "Место доставки:";
                    sheet.Range[$"B{row}:C{row}"].MergeCells = true;
            /// Column right part
                    sheet.Cells[row, 4].HorizontalAlignment = 3;
                    sheet.Cells[row, 4] = $"{contract.Delivery_address}";
                    sheet.Range[$"D{row}:G{row}"].MergeCells = true;
        /// Second row of header table
                    row++;
            /// Column left part
                    sheet.Cells[row, 2].Font.Bold = true;
                    sheet.Cells[row, 2].HorizontalAlignment = 4;
                    sheet.Cells[row, 2] = "Цена доставки:";
                    sheet.Range[$"B{row}:C{row}"].MergeCells = true;
            /// Column right part
                    sheet.Cells[row, 4].HorizontalAlignment = 3;
                    sheet.Cells[row, 4].NumberFormat = "#,#";
                    sheet.Cells[row, 4] = $"{contract.Delivery_amount}";
                    sheet.Range[$"D{row}:G{row}"].MergeCells = true;
        /// Third row of header table
                    row++;
            /// Column left part
                    sheet.Cells[row, 2].Font.Bold = true;
                    sheet.Cells[row, 2].HorizontalAlignment = 4;
                    sheet.Cells[row, 2] = "Дата заказа:";
                    sheet.Range[$"B{row}:C{row}"].MergeCells = true;
            /// Column right part
                    sheet.Cells[row, 4].HorizontalAlignment = 3;
                    sheet.Cells[row, 4] = $"{UnixTimeStampToDateTime(contract.Create_datetime).ToString("d MMM yyyy")} - {UnixTimeStampToDateTime(contract.Return_datetime).ToString("d MMM yyyy")}"; ;
                    sheet.Range[$"D{row}:G{row}"].MergeCells = true;

/// Payments table
    /// Header text
                    row += 2;
                    range = sheet.Range[$"B{row}:G{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.MergeCells = true;
                    range.Font.Size = 18;
                    range.Value = "ОПЛАТЫ ЗАКАЗЧИКА";
    /// Table body
        /// Table column headers row
                    row++;
                    sheet.Cells[row, 3] = "№";
                    sheet.Cells[row, 4] = "Дата оплаты";
                    sheet.Cells[row, 5] = "Сумма оплаты";
                    sheet.Cells[row, 6] = "Способ оплаты";
                    range = sheet.Range[$"C{row}:F{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.Borders.LineStyle = 1;
                    range.Borders.Weight = 3;
        /// Table column value rows
                    row++;
                    int total = 0;
                    int rowNum = 0;
                    foreach (Payment payment in payments)
                    {
                        rowNum++;
                        sheet.Cells[row, 3] = rowNum;
                        sheet.Cells[row, 4] = UnixTimeStampToDateTime(payment.Datetime).ToString("d MMM yyyy");
                        sheet.Cells[row, 5] = payment.Payment_type;
                        sheet.Cells[row, 6].NumberFormat = cellNumberFormat;
                        sheet.Cells[row, 6] = payment.Amount;
                        sheet.Range[$"C{row}:F{row}"].Borders.LineStyle = 1;
                        row++;

                        total += payment.Amount;
                    }
                    /// No payments option/
                    if (payments.Count == 0)
                    {
                        range = sheet.Range[$"C{row}:F{row}"];
                        range.HorizontalAlignment = 3;
                        range.Borders.LineStyle = 1;
                        range.MergeCells = true;
                        range.Value = "Не оплачено";
                    }
    /// Table footer
                    sheet.Cells[row, 5] = "Общее:";
                    sheet.Cells[row, 6].NumberFormat = cellNumberFormat;
                    sheet.Cells[row, 6] = total;
                    range = sheet.Range[$"E{row}:F{row}"];
                    range.Borders.LineStyle = 1;
                    range.Font.Bold = true;
                    range.Font.Size = 16;

/// Orders table
    /// Header text
                    row += 2;
                    range = sheet.Range[$"B{row}:G{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.MergeCells = true;
                    range.Font.Size = 18;
                    range.Value = "ЗАКАЗАННЫЕ ПРОДУКТЫ (за 1 день)";
    /// Table body
        /// Table column headers row
                    row++;
                    sheet.Cells[row, 3] = "Название";
                    sheet.Cells[row, 4] = "Количество";
                    sheet.Cells[row, 5] = "Сумма(1 шт.)";
                    sheet.Cells[row, 6] = "Сумма";
                    range = sheet.Range[$"C{row}:F{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.Borders.LineStyle = 1;
                    range.Borders.Weight = 3;
        /// Table column value rows
                    row++;
                    total = 0;
                    string productName = "";
                    foreach (OrderProduct orderProduct in orderProducts)
                    {
                        sheet.Range[$"E{row}:F{row}"].NumberFormat = cellNumberFormat;
                        sheet.Range[$"C{row}:F{row}"].Borders.LineStyle = 1;
                        productName = dataBaseAC.Products.Where(p => p.Id == orderProduct.Product_id).FirstOrDefault().Name;
                        sheet.Cells[row, 3] = productName;
                        sheet.Cells[row, 4] = orderProduct.Count;
                        sheet.Cells[row, 5] = orderProduct.Price;
                        sheet.Cells[row, 6] = (orderProduct.Count * orderProduct.Price);
                        row++;

                        total += orderProduct.Count * orderProduct.Price;
                    }
                    /// No payments option
                    if (orderProducts.Count == 0)
                    {
                        range = sheet.Range[$"B{row}:E{row}"];
                        range.HorizontalAlignment = 3;
                        range.Borders.LineStyle = 1;
                        range.MergeCells = true;
                        range.Value = "Нет продутов";
                    }
    /// Table footer
                    sheet.Cells[row, 5] = "Общее:";
                    sheet.Cells[row, 6].NumberFormat = cellNumberFormat;
                    sheet.Cells[row, 6] = total;
                    range = sheet.Range[$"E{row}:F{row}"];
                    range.Borders.LineStyle = 1;
                    range.Font.Bold = true;
                    range.Font.Size = 16;

/// Total table part
    /// Header text
                    row += 2;
                    range = sheet.Range[$"B{row}:F{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.MergeCells = true;
                    range.Value = "ИТОГО";
    /// Table body
        /// Table column headers row
                    row++;
                    sheet.Cells[row, 2] = "Кол-во дней";
                    sheet.Cells[row, 3] = "Оплата за 1 день";
                    sheet.Cells[row, 4] = "Цена доставки";
                    sheet.Cells[row, 5] = "Надо оплатить";
                    sheet.Cells[row, 6] = "Оплачено";
                    sheet.Cells[row, 7] = "Долг";
                    range = sheet.Range[$"B{row}:G{row}"];
                    range.Font.Bold = true;
                    range.HorizontalAlignment = 3;
                    range.Borders.LineStyle = 1;
                    range.Borders.Weight = 3;
        /// Table column value rows
                    row++;
                    sheet.Range[$"C{row}:G{row}"].NumberFormat = cellNumberFormat;
                    sheet.Cells[row, 2] = $"{usedDays} + {contract.Used_days}";
                    sheet.Cells[row, 3] = total;
                    sheet.Cells[row, 4] = contract.Delivery_amount;
                    sheet.Cells[row, 5] = shouldPay;
                    sheet.Cells[row, 6] = contract.Paid_amount;
                    if (debt > 0)
                    {
                        sheet.Cells[row, 7].NumberFormat = cellNumberFormat;
                        sheet.Cells[row, 7] = debt;
                    }
                    else
                        sheet.Cells[row, 7] = "Долгов нету";
                    sheet.Range[$"B{row}:G{row}"].Borders.LineStyle = 1;
                    row++;

/// Print settings
                    sheet.Columns["A:H"].EntireColumn.AutoFit();
                    sheet.PageSetup.PrintArea = "$A$1:$H$" + (row + 1).ToString();
                    sheet.PageSetup.Zoom = false;
                    sheet.PageSetup.FitToPagesWide = 1;
                    sheet.PageSetup.FitToPagesTall = false;
                    xlApp.UserControl = true;
                    xlApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (leaseContractDataGrid.SelectedItem == null)
                return;
            MWViewContract mWViewContract = leaseContractDataGrid.SelectedItem as MWViewContract;

            EditLeaseContract editLeaseContract = new EditLeaseContract(dataBaseAC.ReturnedLeaseContracts.Where(l => l.Order_id == mWViewContract.OrderId).FirstOrDefault(), dataBaseAC);
            if (editLeaseContract.ShowDialog() == true)
            {
                dataBaseAC.Entry(editLeaseContract.ReturnedLeaseContract).State = EntityState.Modified;
                dataBaseAC.SaveChanges();
            }

            GetDbToDataGrid();
        }
    }
}
