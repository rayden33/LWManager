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
    /// Логика взаимодействия для ClientsList.xaml
    /// </summary>
    public partial class ClientsList : Window
    {

        ApplicationContext dataBaseAC;
        List<Client> clients;
        public Client SelectedClient { get; private set; }
        public bool isForSelectClient = false;
        public ClientsList(ApplicationContext dbAC)
        {
            InitializeComponent();
            dataBaseAC = dbAC;
            GetDbToDataGrid();
        }

        void GetDbToDataGrid()
        {
            this.DataContext = dataBaseAC.Clients.Local.OrderByDescending(q => q.Last_order_datetime);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClientEditor clientEditor = new ClientEditor(new Client(), dataBaseAC);
            if (clientEditor.ShowDialog() == true)
            {
                Client client = clientEditor.Client;
                dataBaseAC.Clients.Add(client);
                //client.Id++;
                //MessageBox.Show(client.Id.ToString());
                dataBaseAC.SaveChanges();
                GetDbToDataGrid();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            // если ни одного объекта не выделено, выходим
            if (ClientListDG.SelectedItem == null) return;
            // получаем выделенный объект
            Client client = ClientListDG.SelectedItem as Client;
            dataBaseAC.Clients.Remove(client);
            dataBaseAC.SaveChanges();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (ClientListDG.SelectedItem == null) return;
            // получаем выделенный объект
            Client client = ClientListDG.SelectedItem as Client;

            ClientEditor clientEditor = new ClientEditor(new Client
            {
                Id = client.Id,
                Name = client.Name,
                Surname = client.Surname,
                Middle_name = client.Middle_name,
                Pass_number = client.Pass_number,
                Phone_number = client.Phone_number,
                Phone_number2 = client.Phone_number2,
                Address = client.Address
            },dataBaseAC);
            clientEditor.isEditMode = true;
            if (clientEditor.ShowDialog() == true)
            {
                // получаем измененный объект
                client = dataBaseAC.Clients.Find(clientEditor.Client.Id);
                if (client != null)
                {
                    client.Name = clientEditor.Client.Name;
                    client.Surname = clientEditor.Client.Surname;
                    client.Middle_name = clientEditor.Client.Middle_name;
                    client.Pass_number = clientEditor.Client.Pass_number;
                    client.Phone_number = clientEditor.Client.Phone_number;
                    client.Phone_number2 = clientEditor.Client.Phone_number2;
                    client.Address = clientEditor.Client.Address;
                    dataBaseAC.Entry(client).State = EntityState.Modified;
                    dataBaseAC.SaveChanges();
                    GetDbToDataGrid();
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = searchTxtBox.Text;
            List<Client> searchResult = new List<Client>();
            clients = dataBaseAC.Clients.ToList();
            if (text == null || text == "")
            {
                GetDbToDataGrid();
                return;
            }

            foreach (Client tmpClient in clients)
            {
                if (tmpClient.Name != null && tmpClient.Name.Contains(text))
                    searchResult.Add(tmpClient);
                else if (tmpClient.Surname != null && tmpClient.Surname.Contains(text))
                    searchResult.Add(tmpClient);
                else if (tmpClient.Middle_name != null && tmpClient.Middle_name.Contains(text))
                    searchResult.Add(tmpClient);
                else if (tmpClient.Phone_number != null && tmpClient.Phone_number.Contains(text))
                    searchResult.Add(tmpClient);
                else if (tmpClient.Pass_number != null && tmpClient.Pass_number.Contains(text))
                    searchResult.Add(tmpClient);
            }

            this.DataContext = searchResult;

        }

        private void ClientListDG_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void ClientListDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientListDG.SelectedItem == null) return;
                SelectedClient = ClientListDG.SelectedItem as Client;
            if (SelectedClient.Is_blocked == 1)
                blockingBtn.Content = "Разблокировать";
            else
                blockingBtn.Content = "Заблокировать";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (ClientListDG.SelectedItem == null) return;
            // получаем выделенный объект
            Client client = ClientListDG.SelectedItem as Client;
            client.Is_blocked ^= 1;
            if (SelectedClient.Is_blocked == 1)
                blockingBtn.Content = "Разблокировать";
            else
                blockingBtn.Content = "Заблокировать";
        }

        private void ClientListDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientListDG.SelectedItem == null) return;
            SelectedClient = ClientListDG.SelectedItem as Client;

            if (!isForSelectClient) return;
            if (SelectedClient.Is_blocked == 1)
            {
                MessageBox.Show("Этот пользователь заблокирован!");
                return;
            }
            if (MessageBox.Show($"Хотите выбрать: {SelectedClient.Surname} {SelectedClient.Name}",
                "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                this.DialogResult = true;
        }

        private void AutoSizeWindowAndElements()
        {
            DataGrid DataGridForResize = ClientListDG;
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

        private void ClientListDG_Loaded(object sender, RoutedEventArgs e)
        {
            AutoSizeWindowAndElements();
        }

        private void ClientOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ClientListDG.SelectedItem == null) return;
            SelectedClient = ClientListDG.SelectedItem as Client;

            ClientOrderList clientOrderList = new ClientOrderList(dataBaseAC, SelectedClient);
            clientOrderList.Show();
        }

        private void ClientDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientListDG.SelectedItem == null) return;
            SelectedClient = ClientListDG.SelectedItem as Client;

            DeleteClientWithGlobalChanges(SelectedClient);
        }

        private void DeleteClientWithGlobalChanges(Client client)
        {
            List<ArchiveLeaseContract> archiveLeaseContracts = new List<ArchiveLeaseContract>();
            List<LeaseContract> leaseContracts = new List<LeaseContract>();
            List<OrderProduct> orderProducts = new List<OrderProduct>();
            List<Payment> payments = new List<Payment>();
            List<ReturnProduct> returnProducts = new List<ReturnProduct>();
            List<ReturnedLeaseContract> returnedLeaseContracts = new List<ReturnedLeaseContract>();
            archiveLeaseContracts.AddRange(dataBaseAC.ArchiveLeaseContracts.Where(q => q.Client_id == SelectedClient.Id).ToList());
            leaseContracts.AddRange(dataBaseAC.LeaseContracts.Where(q => q.Client_id == SelectedClient.Id).ToList());
            returnedLeaseContracts.AddRange(dataBaseAC.ReturnedLeaseContracts.Where(q => q.Client_id == SelectedClient.Id).ToList());


            if (leaseContracts.Count > 0)
            {
                MessageBox.Show("Сперва закройте все активные заказы этого клиента на главном окне!!!");
                return;
            }
            
            if (returnedLeaseContracts.Count > 0)
            {
                MessageBox.Show("Закройте все активные заказы этого клиента на окне возврата!!!");
                return;
            }

            if (MessageBox.Show("Точно хотите удалить?", "Удалить?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;


            /// ArchiveLeaseContract cleaning
            foreach (ArchiveLeaseContract contract in archiveLeaseContracts)
            {

                /// Order cleaning
                orderProducts.Clear();
                orderProducts.AddRange(dataBaseAC.OrderProducts.Where(q => q.Order_id == contract.Order_id).ToList());
                foreach(OrderProduct orderProduct in orderProducts)
                {
                    dataBaseAC.OrderProducts.Remove(orderProduct);
                    dataBaseAC.SaveChanges();
                }

                /// Payment cleaning
                payments.Clear();
                payments.AddRange(dataBaseAC.Payments.Where(q => q.Order_id == contract.Order_id).ToList());
                foreach (Payment payment in payments)
                {
                    dataBaseAC.Payments.Remove(payment);
                    dataBaseAC.SaveChanges();
                }

                returnProducts.Clear();
                returnProducts.AddRange(dataBaseAC.ReturnProducts.Where(q => q.Order_id == contract.Order_id).ToList());
                foreach(ReturnProduct returnProduct in returnProducts)
                {
                    dataBaseAC.ReturnProducts.Remove(returnProduct);
                    dataBaseAC.SaveChanges();
                }


                dataBaseAC.ArchiveLeaseContracts.Remove(contract);
                dataBaseAC.SaveChanges();
            }

            dataBaseAC.Clients.Remove(client);
            dataBaseAC.SaveChanges();
            GetDbToDataGrid();
        }


        private void ClientPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please install xlsx format");
        }


        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }
    }
}
