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
    /// Логика взаимодействия для ProductList.xaml
    /// </summary>
    public partial class ProductList : Window
    {
        ApplicationContext dataBaseAC;
        List<Product> products = new List<Product>();
        List<Product> allProduct = new List<Product>();
        public Product SelectedProduct { get; private set; }
        public bool isForSelectProduct = false;
        public ProductList(ApplicationContext dbAC, bool forReferences)
        {
            InitializeComponent();
            dataBaseAC = dbAC;
            isForSelectProduct = forReferences;
            //DataContext = dataBaseAC.Products.ToList();
            GetDbToDataGrid();

            
        }

        void GetDbToDataGrid()
        {
            /*List<Product> tmpProducts = new List<Product>();
            foreach (Product pro in dataBaseAC.Products.Local)
            {
                if (pro.Reference_id == 0)
                    tmpProducts.Add(pro);
            }
            this.DataContext = tmpProducts;*/

            this.DataContext = dataBaseAC.Products.Local.Where(p => p.Reference_id == 0).ToList();
            int tempSum = 0;
            

            foreach (Product prd in dataBaseAC.Products.Local)
            {
                tempSum = 0;
                switch (prd.Id)
                {
                    case 5:
                        bLesaCountLbl.Content = prd.Count;
                        bLesaOrderCountLbl.Content = getCurrentInOrderProductsCount(5);
                        bLesaSumCountLbl.Content = ((int)bLesaCountLbl.Content + (int)bLesaOrderCountLbl.Content);
                        break;
                    case 6:
                        mLesaCountLbl.Content = prd.Count;
                        mLesaOrderCountLbl.Content = getCurrentInOrderProductsCount(6);
                        mLesaSumCountLbl.Content = ((int)mLesaCountLbl.Content + (int)mLesaOrderCountLbl.Content);
                        break;
                    case 1:
                        bLesaSubCountLbl2.Content = prd.Count;
                        bLesaSubOrderCountLbl2.Content = getCurrentInOrderProductsCount(1);
                        bLesaSubSumCountLbl2.Content = ((int)bLesaSubCountLbl2.Content + (int)bLesaSubOrderCountLbl2.Content);
                        break;
                    case 2:
                        bLesaSubCountLbl1.Content = prd.Count;
                        bLesaSubOrderCountLbl1.Content = getCurrentInOrderProductsCount(2);
                        bLesaSubSumCountLbl1.Content = ((int)bLesaSubCountLbl1.Content + (int)bLesaSubCountLbl1.Content);
                        break;
                    case 3:
                        bLesaSubCountLbl3.Content = prd.Count;
                        bLesaSubOrderCountLbl3.Content = getCurrentInOrderProductsCount(3);
                        bLesaSubSumCountLbl3.Content = ((int)bLesaSubCountLbl3.Content + (int)bLesaSubOrderCountLbl3.Content);
                        break;
                    case 7:
                        mLesaSubCountLbl1.Content = prd.Count;
                        //bLesaOrderCountLbl.Content = dataBaseAC.OrderProducts.Where(op => op.Product_id == 5).Sum(op => op.Count);
                        mLesaSubOrderCountLbl1.Content = getCurrentInOrderProductsCount(7);
                        mLesaSubSumCountLbl1.Content = ((int)mLesaSubCountLbl1.Content + (int)mLesaSubOrderCountLbl1.Content);
                        break;
                    case 8:
                        mLesaSubCountLbl2.Content = prd.Count;
                        //bLesaOrderCountLbl.Content = dataBaseAC.OrderProducts.Where(op => op.Product_id == 5).Sum(op => op.Count);
                        mLesaSubOrderCountLbl2.Content = getCurrentInOrderProductsCount(8);
                        mLesaSubSumCountLbl2.Content = ((int)mLesaSubCountLbl2.Content + (int)mLesaSubOrderCountLbl2.Content);
                        break;
                    case 9:
                        mLesaSubCountLbl3.Content = prd.Count;
                        //bLesaOrderCountLbl.Content = dataBaseAC.OrderProducts.Where(op => op.Product_id == 5).Sum(op => op.Count);
                        mLesaSubOrderCountLbl3.Content = getCurrentInOrderProductsCount(9);
                        mLesaSubSumCountLbl3.Content = ((int)mLesaSubCountLbl3.Content + (int)mLesaSubOrderCountLbl3.Content);
                        break;
                    case 4:
                        kolesikCountLbl.Content = prd.Count;
                        //bLesaOrderCountLbl.Content = dataBaseAC.OrderProducts.Where(op => op.Product_id == 5).Sum(op => op.Count);
                        kolesikOrderCountLbl.Content = getCurrentInOrderProductsCount(4);
                        kolesikSumCountLbl.Content = ((int)kolesikCountLbl.Content + (int)kolesikOrderCountLbl.Content);
                        break;

                }
            }
        }

        private int getCurrentInOrderProductsCount(int productId)
        {
            int count = 0;
            string tempStr1 = "";
            string tempStr2 = "";

            if (dataBaseAC.OrderProducts.Where(op => op.Product_id == productId).Count() > 0)
                tempStr1 = dataBaseAC.OrderProducts.Where(op => op.Product_id == productId).Sum(op => op.Count).ToString();
            else
                tempStr1 = "0";

            if (dataBaseAC.ReturnProducts.Where(op => op.Product_id == productId).Count() > 0)
                tempStr2 = dataBaseAC.ReturnProducts.Where(op => op.Product_id == productId).Sum(op => op.Count).ToString();
            else
                tempStr2 = "0";

            count = Convert.ToInt32(tempStr1);
            count -= Convert.ToInt32(tempStr2);

            return count;
        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = searchTxtBox.Text;
            List<Product> searchResult = new List<Product>();
            if (isForSelectProduct)
            {
                foreach (Product pro in dataBaseAC.Products.Local)
                {
                    if (pro.Reference_id == 0)
                        products.Add(pro);
                }
            }
            else
                products = dataBaseAC.Products.Local.ToList();

            if (text == null || text == "")
            {
                GetDbToDataGrid();
                return;
            }

            foreach (Product tmpProduct in products)
            {
                if (tmpProduct.Name.Contains(text))
                    searchResult.Add(tmpProduct);
            }

            this.DataContext = searchResult;

        }


        private void ProductListDG_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void ProductListDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ProductListDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductListDG.SelectedItem == null) return;
            SelectedProduct = ProductListDG.SelectedItem as Product;

            if (!isForSelectProduct) return;
            if (MessageBox.Show($"Хотите выбрать: {SelectedProduct.Name}",
                "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                this.DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductEditor productEditor = new ProductEditor(new Product());
            if (productEditor.ShowDialog() == true)
            {
                Product product = productEditor.Product;
                dataBaseAC.Products.Add(product);
                dataBaseAC.SaveChanges();
            }
            GetDbToDataGrid();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // если ни одного объекта не выделено, выходим
            if (ProductListDG.SelectedItem == null) return;
            // получаем выделенный объект
            Product product = ProductListDG.SelectedItem as Product;

            ProductEditor productEditor = new ProductEditor(new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count,
                Reference_id = product.Reference_id
            });

            if (productEditor.ShowDialog() == true)
            {
                // получаем измененный объект
                product = dataBaseAC.Products.Find(productEditor.Product.Id);
                if (product != null)
                {
                    product.Name = productEditor.Product.Name;
                    product.Price = productEditor.Product.Price;
                    product.Count = productEditor.Product.Count;
                    dataBaseAC.Entry(product).State = EntityState.Modified;
                    dataBaseAC.SaveChanges();
                }
            }
            GetDbToDataGrid();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void AutoSizeWindowAndElements()
        {
            DataGrid DataGridForResize = ProductListDG;
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

        private void ProductListDG_Loaded(object sender, RoutedEventArgs e)
        {
            //AutoSizeWindowAndElements();
        }

        private void editStdLesa_Click(object sender, RoutedEventArgs e)
        {
            /// product id = 5 standart lesa id
            Product product = dataBaseAC.Products.Where(q => q.Id == 5).FirstOrDefault();
            MainProductEditor mainProductEditor = new MainProductEditor(dataBaseAC, new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count,
                Reference_id = product.Reference_id
            });

            if (mainProductEditor.ShowDialog() == true)
            {
                // получаем измененный объект
                product = dataBaseAC.Products.Find(mainProductEditor.Product.Id);
                if (product != null)
                {
                    product.Name = mainProductEditor.Product.Name;
                    product.Price = mainProductEditor.Product.Price;
                    product.Count = mainProductEditor.Product.Count;
                    dataBaseAC.Entry(product).State = EntityState.Modified;
                    dataBaseAC.SaveChanges();
                }

                for(int i=0;i<mainProductEditor.SubProducts.Count; i++)
                {
                    product = dataBaseAC.Products.Find(mainProductEditor.SubProducts[i].Id);
                    if (product != null)
                    {
                        product.Name = mainProductEditor.SubProducts[i].Name;
                        product.Price = mainProductEditor.SubProducts[i].Price;
                        product.Count = mainProductEditor.SubProducts[i].Count;
                        dataBaseAC.Entry(product).State = EntityState.Modified;
                        dataBaseAC.SaveChanges();
                    }
                }
            }
            GetDbToDataGrid();

        }

        private void editNotStdLesa_Click(object sender, RoutedEventArgs e)
        {
            /// product id = 6 no standart lesa id
            Product product = dataBaseAC.Products.Where(q => q.Id == 6).FirstOrDefault();
            MainProductEditor mainProductEditor = new MainProductEditor(dataBaseAC, new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count,
                Reference_id = product.Reference_id
            });

            if (mainProductEditor.ShowDialog() == true)
            {
                // получаем измененный объект
                product = dataBaseAC.Products.Find(mainProductEditor.Product.Id);
                if (product != null)
                {
                    product.Name = mainProductEditor.Product.Name;
                    product.Price = mainProductEditor.Product.Price;
                    product.Count = mainProductEditor.Product.Count;
                    dataBaseAC.Entry(product).State = EntityState.Modified;
                    dataBaseAC.SaveChanges();
                }

                for (int i = 0; i < mainProductEditor.SubProducts.Count; i++)
                {
                    product = dataBaseAC.Products.Find(mainProductEditor.SubProducts[i].Id);
                    if (product != null)
                    {
                        product.Name = mainProductEditor.SubProducts[i].Name;
                        product.Price = mainProductEditor.SubProducts[i].Price;
                        product.Count = mainProductEditor.SubProducts[i].Count;
                        dataBaseAC.Entry(product).State = EntityState.Modified;
                        dataBaseAC.SaveChanges();
                    }
                }
            }
            GetDbToDataGrid();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            /// product id = 4 kolesiki id
            Product product = dataBaseAC.Products.Where(q => q.Id == 4).FirstOrDefault();
            MainProductEditor mainProductEditor = new MainProductEditor(dataBaseAC, new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count,
                Reference_id = product.Reference_id
            });

            if (mainProductEditor.ShowDialog() == true)
            {
                // получаем измененный объект
                product = dataBaseAC.Products.Find(mainProductEditor.Product.Id);
                if (product != null)
                {
                    product.Name = mainProductEditor.Product.Name;
                    product.Price = mainProductEditor.Product.Price;
                    product.Count = mainProductEditor.Product.Count;
                    dataBaseAC.Entry(product).State = EntityState.Modified;
                    dataBaseAC.SaveChanges();
                }
            }
            GetDbToDataGrid();
        }
    }
}
